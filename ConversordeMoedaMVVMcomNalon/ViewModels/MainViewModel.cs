using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Globalization;
using System.Runtime.CompilerServices;
using ConversordeMoedaMVVMcomNalon.Models;


namespace ConversordeMoedaMVVMcomNalon.ViewModels
{

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly RateTable _rates = new();

        public IList<string> Currencies { get; }

        string? _amountText;

        public string? AmountText
        {
            get => _amountText;
            set
            {
                if (value == _amountText) return;
                _amountText = value;
                OnPropertyChanged();
                ((Command)ConvertCommand).ChangeCanExecute();
            }
        }

        string _from = "USD";
        public string From
        {
            get => _from;
            set
            {
                if (_from == value) return;
                _from = value;
                OnPropertyChanged();
                ((Command)ConvertCommand).ChangeCanExecute();
            }
        }

        string _to = "BRL";
        public string To
        {
            get => _to;
            set
            {
                if (_to == value) return;
                _to = value;
                OnPropertyChanged();
                ((Command)ConvertCommand).ChangeCanExecute();
            }
        }

        string _resultText = "--";
        public string ResultText
        {
            get => _resultText;
            set
            {
                if (_resultText != value)
                { _resultText = value; OnPropertyChanged(); }
            }
        }

        public ICommand ConvertCommand { get; }
        public ICommand SwapCommand { get; }
        public ICommand ClearCommand { get; }

        readonly CultureInfo _pt = new("pt-BR");

        public MainViewModel()
        {
            Currencies = _rates.GetCurrencies().ToList();

            ConvertCommand = new Command(DoConvert, CanConvert);
            SwapCommand = new Command(DoSwap);
            ClearCommand = new Command(DoClear);
        }

        bool CanConvert()
        {
            if (string.IsNullOrWhiteSpace(AmountText)) return false;
            return TryParseAmount(AmountText, out _);
        }


        void DoConvert()
        {
            if (!TryParseAmount(AmountText, out var amount))
            {
                ResultText = "Valor inválido";
                return;
            }
            if (!_rates.Supports(From) || !_rates.Supports(To))
            {
                ResultText = "Moeda inválida";
                return;
            }

            var result = _rates.Convert(amount, From, To);
            ResultText = string.Format(_pt, "{0:N2} {1} = {2:N2} {3}", amount, From, result, To);
        }


        void DoSwap()
        {
            (From, To) = (To, From);
            ResultText = "--";
        }

        void DoClear()
        {
            AmountText = string.Empty;
            ResultText = "--";
        }

        bool TryParseAmount(string? text, out decimal amount)
        {
            amount = 0m;
            if (string.IsNullOrWhiteSpace(text)) return false;

            var s = text.Trim();
            if (!decimal.TryParse(s, NumberStyles.Number, _pt, out amount)) return true;

            s = s.Replace(".", ",");
            return decimal.TryParse(s, NumberStyles.Number, _pt, out amount);
        }
    }
}

