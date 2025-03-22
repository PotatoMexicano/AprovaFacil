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
  });

  const {formState: {isSubmitSuccessful}} = form;

  useEffect(() => {
    if (isSubmitSuccessful) {
      // Após o sucesso e o reset, podemos zerar estados adicionais aqui
        setDisplayValue("");
        setCurrencyWords("");
      // Opcional: Redefinir outros estados fora do formulário, se existirem
      // Exemplo: setSomeExternalState(initialValue);
    }
  }, [isSubmitSuccessful]);

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
      <FormField
        control={form.control}
        name="amount"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Valor</FormLabel>
            <FormControl>
              <div className="relative">
                <Input
                  type="text"
                  inputMode="decimal"
                  value={displayValue || formatCurrency(`${field.value / 100 || 0}`)}
                  onChange={(e) => {
                    const inputValue = e.target.value;
                    setDisplayValue(inputValue);

                    if (!inputValue) {
                      field.onChange(undefined);
                      setCurrencyWords(currencyToWords.convert(0));
                      return;
                    }

                    try {
                      const numericValueInReais = parseCurrency(inputValue);
                      if (!isNaN(numericValueInReais)) {
                        const valueInCents = Math.round(numericValueInReais * 100);
                        field.onChange(valueInCents);
                        setCurrencyWords(currencyToWords.convert(numericValueInReais));
                      }
                    } catch (error) {
                      console.log('Erro ao converter valor:', error);
                    }
                  }}
                  onBlur={() => {
                    const valueInCents = field.value || 0;
                    const valueInReais = valueInCents / 100;
                    const formatted = formatCurrency(`${valueInReais}`);
                    setDisplayValue(formatted);
                    form.setValue('amount', valueInCents);
                    setCurrencyWords(currencyToWords.convert(valueInReais));
                  }}
                  placeholder="Ex: R$ 1,00"
                  className="w-full pr-8" // Adiciona padding à direita para o ícone
                />
                <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                  <DollarSign className="w-4 h-4 text-foreground opacity-50" />
                </div>
              </div>
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
      <small className="text-gray-500 text-sm block mt-1">{currencyWords}</small>
    </div>
  );
};

export default CurrencyFormField;