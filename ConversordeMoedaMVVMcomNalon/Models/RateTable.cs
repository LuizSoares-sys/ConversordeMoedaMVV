// na model, estará toda a tabela de cambio ficticia + regras de conversao de moedas

namespace ConversordeMoedaMVVMcomNalon.Models
{
    public class RateTable
    {
        private readonly Dictionary<string, decimal>
            _toBRL = new()
        {
            { "USD", 5.60m },  //real brasileiro
            { "EUR", 6.10m }, // Dolar americano
            { "BRL", 1.00m } // Euro
        };
        //propriedade para leitura externa de dicionario de taxas de cambio

        public IReadOnlyDictionary<string, decimal> ToBRL => _toBRL;  //faz qualquer pessoa acessar o dicionario, mas nao alterar

        public IEnumerable<string> GetCurrencies() => _toBRL.Keys.OrderBy(k => k);
        //Public IEnumerable: retorna as chaves do dicionario, ou seja, as moedas disponiveis
        //strings: indica que a coleção contem elementosdo tipo string
        //GetCurrencies: nome do método que retorna a coleção de strings

        public bool Supports(string Code) => _toBRL.ContainsKey(Code);
        // Verifica se a moeda é suportada, retornando true ou false

        //Método principal de conversao de moedas   

        public decimal Convert(decimal amount, string from, string to)
        {
            if (!Supports(from) || !Supports(to)) return 0m;
            //se a moeda nao for suportada, retorna 0

            if (from == to) return amount;
            //se as moedas forem iguais, retorna o valor original

            var brl = amount * _toBRL[from];
            //converte o valor para BRL (Real brasileiro) como moeda intermediaria/padrao (se for real ela verifica se é igual e converte, se for euro, converte primeiro pra brl e dps euro)
            return brl / _toBRL[to];

        }
    }

}