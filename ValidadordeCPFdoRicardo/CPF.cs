using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidadordeCPFdoRicardo
{
    public class CPF
    {
        public string? Numero { get; set; }
        public string? DbCPF { get; set; }

        //validar o cpf

        public static bool Validar(string cpf)
        {
            bool cpfValido = true;

            //verificar se tem 11 digitos
            if (cpf.Length != 11)
                cpfValido = false;

            else
            {
                //verificar caracteres validos (numericos)
                for (int i = 0; i < cpf.Length; i++)
                {
                    if (!char.IsDigit(cpf[i]))
                    {
                        cpfValido = false;
                        break;
                    }

                }

            }

            //verificar se sao numeros identicos

            switch (cpf)
            {
                case "00000000000":
                    return false;
                case "11111111111":
                    return false;
                case "2222222222":
                    return false;
                case "33333333333":
                    return false;
                case "44444444444":
                    return false;
                case "55555555555":
                    return false;
                case "66666666666":
                    return false;
                case "77777777777":
                    return false;
                case "88888888888":
                    return false;
                case "99999999999":
                    return false;
            }

            //verificar digito de controle do cpf
            if (cpfValido)
            {
                var j = 0;
                var d1 = 0;
                var d2 = 0;

                //validar o primeiro digito de controle
                for (int i = 10; i > 1; i--)
                {
                    d1 += Convert.ToInt32(cpf.Substring(j, 1)) * i;
                    j++;
                }

                //modulo ou resto da divisao
                d1 = (d1 * 10) % 11;
                if (d1 == 10) //se igual a 10, vira 0
                    d1 = 0;

                //verificar se o primeiro digito verificador deu certo na pos 9
                if (d1 != Convert.ToInt32(cpf.Substring(j, 1)))
                    cpfValido = false;

                //validar o segundo digito
                if (cpfValido)
                {
                    j = 0;
                    for (int i = 11; i > 1; i--)
                    {
                        d2 += Convert.ToInt32(cpf.Substring(j, 1)) * i;
                        j++;
                    }

                    //resto ou modulo da divisao
                    d2 = (d2 * 10) % 11;
                    if (d2 == 10)
                        d2 = 0;

                    // verificar se o segundo digito bateu na pos 10
                    if (d2 != Convert.ToInt32(cpf.Substring(10, 1)))
                        cpfValido = false;

                }

               
            }


            return cpfValido;

        }

    }
}
