import { useEffect, useState } from "react";
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from "./form";
import { Input } from "./input";
import { formatCurrency, parseCurrency } from "@/lib/utils";
import { ToWords } from 'to-words';
import { DollarSign } from "lucide-react";

const CurrencyFormField = ({ form }) => {
  const [displayValue, setDisplayValue] = useState("")
  const [currencyWords, setCurrencyWords] = useState("") // Estado para o valor por extenso

  const currencyToWords = new ToWords({
    localeCode: "pt-BR",
    converterOptions: {
      currency: true,
      ignoreDecimal: false,
      ignoreZeroCurrency: false,
      doNotAddOnly: false,
      currencyOptions: {
        name: "real",
        plural: "reais",
        symbol: "R$",
        fractionalUnit: {
          name: "centavo",
          plural: "centavos",
          symbol: "",
        },
      },
    },
  })

  // Inicializa o texto por extenso quando o componente monta
  useEffect(() => {
    // Verifica se já existe um valor no formulário
    const initialValue = form.getValues("amount")
    if (initialValue !== undefined) {
      setCurrencyWords(currencyToWords.convert(initialValue))
      setDisplayValue(formatCurrency(`${initialValue}`))
    } else {
      setCurrencyWords(currencyToWords.convert(0))
    }
  }, [form.getValues, currencyToWords.convert])

  return (
    <div>

      <div className="relative">
        <FormField
          control={form.control}
          name="amount"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Valor</FormLabel>
              <FormControl>
                <Input
                  type="text"
                  inputMode="decimal"
                  value={displayValue || formatCurrency(`${field.value || undefined}`)}
                  onChange={(e) => {
                    const inputValue = e.target.value

                    // Mantém o valor como digitado para uma melhor experiência do usuário
                    setDisplayValue(inputValue)

                    // Se o campo estiver vazio, define como zero
                    if (!inputValue) {
                      field.onChange()
                      setCurrencyWords(currencyToWords.convert(0))
                      return
                    }

                    // Só processa se for um valor que pode ser convertido para moeda
                    try {
                      const numericValueInReais = parseCurrency(inputValue)

                      // Só atualiza o formulário se for um número válido
                      if (!isNaN(numericValueInReais)) {
                        // Converte para centavos como inteiro
                        const valueInCents = Math.round(numericValueInReais * 100)

                        // Exemplo: 200.91 -> 20091
                        field.onChange(valueInCents)

                        // O texto por extenso continua usando o valor em reais
                        setCurrencyWords(currencyToWords.convert(numericValueInReais || 0))
                      }
                    } catch (error) {
                      // Se houver erro na conversão, não atualiza o valor
                      console.log("Erro ao converter valor:", error)
                    }
                  }}
                  onBlur={() => {
                    // Garante que temos um número válido (em centavos)
                    const valueInCents = field.value || 0

                    // Converte para reais para formatação e exibição
                    const valueInReais = valueInCents / 100

                    // Formata o valor ao perder o foco
                    const formatted = formatCurrency(`${valueInReais}`)
                    setDisplayValue(formatted)

                    // Atualiza o formulário com o valor em centavos
                    form.setValue("valor", valueInCents)

                    // Atualiza o texto por extenso (usando o valor em reais)
                    setCurrencyWords(currencyToWords.convert(valueInReais))
                  }}
                  placeholder="Ex: R$ 1,00"
                  className="w-full" />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="absolute right-3 top-[73%] -translate-y-1/2 h-4 w-4">
          <DollarSign className="w-4 h-4 text-foreground opacity-50" />
        </div>
      </div>
      <small className="text-gray-500 text-sm block mt-1">{currencyWords}</small>
    </div>
  );
};

export default CurrencyFormField;