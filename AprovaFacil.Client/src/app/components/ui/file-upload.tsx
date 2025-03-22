"use client"

import React, { useEffect } from "react"

import { FaFile, FaFileExcel, FaFilePdf, FaFileWord } from "react-icons/fa";

import { type ChangeEvent, type DragEvent, forwardRef, useRef, useState } from "react"
import { Button } from "./button"
import { cn } from "@/lib/utils"
import { XIcon, type LucideIcon } from "lucide-react"
import { useToast } from "@/hooks/use-toast";

interface FileUploadProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  value?: File;
  icon: LucideIcon;
  onChange: (file: File | null) => void;
}

const FileUpload = forwardRef<HTMLInputElement, FileUploadProps>(
  ({ className, label = "Selecionar arquivo", value, onChange, required, icon: Icon, ...props }, ref) => {
    const [isDragging, setIsDragging] = useState(false)
    const [fileName, setFileName] = useState<string | null>(null)
    const fileInputRef = useRef<HTMLInputElement>(null);
    const { toast } = useToast();

    useEffect(() => {
      if (value instanceof File) {
        setFileName(value.name);
      } else {
        setFileName(null); // Limpa o fileName quando o value é null/undefined
        if (fileInputRef.current) {
          fileInputRef.current.value = ''; // Limpa o input nativo
        }
      }
    }, [value]);

    const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0] || null; // Pega o primeiro arquivo ou null
      if (file) {
        const extension = file.name.split('.').pop()?.toLowerCase();
        // Verifica se a extensão é permitida
        if (extension && ['pdf', 'docx', 'xlsx'].includes(extension)) {
          onChange(file);
          setFileName(file ? file.name : ''); // Atualiza o estado do nome do arquivo
        } else {
          toast({
            title: "Arquivo não permitido",
            description: "Apenas arquivos PDF, DOCX e XLSX são permitidos."
          });
          if (fileInputRef.current) {
            fileInputRef.current.value = ''; // Limpa o input se o arquivo for inválido
          }
        }
      }
    }

    const getFileIcon = (fileName: string | null) => {
      if (!fileName) return <FaFile className="h-4 w-4 text-muted-foreground" />;

      const extension = fileName.split('.').pop()?.toLowerCase();

      switch (extension) {
        case 'pdf':
          return <FaFilePdf className="h-4 w-4 text-muted-foreground" />;
        case 'docx':
          return <FaFileWord className="h-4 w-4 text-muted-foreground" />;
        case 'xlsx':
          return <FaFileExcel className="h-4 w-4 text-muted-foreground" />;
        default:
          return <FaFile className="h-4 w-4 text-muted-foreground" />; // Fallback (não deve ocorrer)
      }
    };

    const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
      e.preventDefault()
      e.stopPropagation()
      setIsDragging(true)
    }

    const handleDragLeave = (e: DragEvent<HTMLDivElement>) => {
      e.preventDefault()
      e.stopPropagation()
      setIsDragging(false)
    }

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
      e.preventDefault()
      e.stopPropagation()
      setIsDragging(false)

      const file = e.dataTransfer.files?.[0] || null
      if (file) {
        setFileName(file.name)
        onChange(file)
      }
    }

    const clearFile = () => {
      setFileName(null)
      onChange(null);

      if (fileInputRef.current) {
        fileInputRef.current.value = ''; // Reseta o valor do input
      }
    }

    return (
      <div className={cn("flex flex-col gap-2", className)}>
        <div
          className={cn(
            "relative flex min-h-[150px] w-full cursor-pointer flex-col items-center justify-center rounded-md border border-dashed border-foreground/20 bg-background p-4 transition-colors hover:bg-muted/50",
            isDragging && "border-primary bg-muted/50",
            fileName && "border-solid",
          )}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
        >
          <input
            type="file"
            className="absolute inset-0 cursor-pointer opacity-0"
            onChange={handleFileChange}
            ref={fileInputRef}
            required={required}
            {...props}
            accept=".pdf,.docx,.xlsx" // Restringe os tipos no input
          />

          {fileName ? (
            <div className="flex w-full flex-col items-center gap-2">
              <div className="flex w-full items-center justify-between rounded-md border bg-background p-2">
                <div className="flex items-center gap-2">
                  {getFileIcon(fileName)}
                  <p>Arquivo: {fileName || 'Nenhum arquivo selecionado'}</p>
                </div>
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="h-6 w-6 p-0 z-10"
                  onClick={(e) => {
                    e.preventDefault()
                    e.stopPropagation()
                    clearFile()
                  }}
                >
                  <XIcon className="h-4 w-4" />
                  <span className="sr-only">Remover arquivo</span>
                </Button>
              </div>
              <p className="text-xs text-muted-foreground">Clique ou arraste para substituir</p>
            </div>
          ) : (
            <div className="flex flex-col items-center gap-2 text-center">
              <div className="rounded-full bg-muted p-2">
                {React.createElement(Icon, { className: "h-4 w-4 text-muted-foreground" })}
                {/* <UploadIcon className="h-4 w-4 text-muted-foreground" /> */}
              </div>
              <div className="flex flex-col space-y-1">
                <p className="text-sm font-medium">{label}</p>
                <p className="text-xs text-muted-foreground">Arraste e solte ou clique para selecionar</p>
              </div>
            </div>
          )}
        </div>
      </div>
    )
  },
)

FileUpload.displayName = "FileUpload"

export default FileUpload

