import { motion } from "framer-motion";
import { Button } from "./button";
import { Check, Loader2 } from "lucide-react";

interface Props {
  isLoading: boolean;
  isSuccess: boolean | undefined;
  loadingText?: string;
  successText?: string;
  defaultText?: string;
}

export default function ButtonSuccess({isLoading, isSuccess, defaultText, loadingText, successText}: Props) {
  return (
    <Button
      disabled={isLoading}
      className={`p-5 ${isSuccess ? "bg-green-500 hover:bg-green-600 text-white" : ""}`}
      variant={"default"}
      type="submit"
      >
      {isLoading
        ? (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex items-center gap-2">
            <Loader2 className="w-4 h-4 animate-spin" />
            <span>{loadingText}</span>
          </motion.div>
        )
        : isSuccess
          ? (
            <motion.div className="flex items-center gap-2" initial={{ opacity: 0 }} animate={{ opacity: 1 }}>
              <Check className="w-5 h-5" />
              <span>{successText}</span>
            </motion.div>
          )
          : (
            <span>{defaultText}</span>
          )
      }
    </Button>
  )
}