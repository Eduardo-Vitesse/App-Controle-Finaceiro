using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using AppControleFinanceiro.Utils.FixBugs;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views
{
    public partial class TransactionAdd : ContentPage
    {
        private ITransactionRepository _repository;
	    public TransactionAdd(ITransactionRepository repository)
	    {
            _repository = repository;
		    InitializeComponent();
	    }

        private async void TapGestureRecognizer_Add(object sender, TappedEventArgs e)
        {
            KeyboardBugs.HideKeyboard();
            await Navigation.PopModalAsync();
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (isValidData() == false)
                return;

            SaveTransactionInDatabase();

            KeyboardBugs.HideKeyboard();

            await Navigation.PopModalAsync();
            WeakReferenceMessenger.Default.Send<string>("Registro cadastrado no DB");
        }

        private void SaveTransactionInDatabase()
        {
            Transaction transaction = new Transaction()
            {
                Type = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expenses,
                Name = EntryName.Text,
                Date = EntryDate.Date,
                Value = Math.Abs(double.Parse(EntryValue.Text)),
            };

            _repository.Add(transaction);
        }

        private bool isValidData()
        {
            bool valid = true;
            StringBuilder sb = new StringBuilder();

            if(string.IsNullOrEmpty(EntryName.Text) || string.IsNullOrWhiteSpace(EntryName.Text))
            {
                sb.AppendLine("O campo 'Nome' deve ser preenchido");
                valid = false;
            }

            if (string.IsNullOrEmpty(EntryValue.Text) || string.IsNullOrWhiteSpace(EntryValue.Text))
            {
                sb.AppendLine("O campo 'Valor' deve ser preenchido");
                valid = false;
            }

            double result;
            if (!string.IsNullOrEmpty(EntryValue.Text) && !double.TryParse(EntryValue.Text, out result))
            {
                sb.AppendLine("O campo 'Valor' é inválido...");
                valid = false;
            }

            if (valid == false)
            {
                LabelError.IsVisible = true;
                LabelError.Text = sb.ToString();
            }

            return valid;
        }
    }

}
