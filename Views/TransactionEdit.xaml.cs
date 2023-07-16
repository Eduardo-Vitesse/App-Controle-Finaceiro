using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using AppControleFinanceiro.Utils.FixBugs;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views;

public partial class TransactionEdit : ContentPage
{
	private Transaction _transaction;
    private ITransactionRepository _repository;

    public TransactionEdit(ITransactionRepository repository)
	{
        _repository = repository;
        InitializeComponent();
	}

	public void SetTransactionToEdit(Transaction transaction)
	{
		_transaction = transaction;

        if (transaction.Type == TransactionType.Income)
        {
            RadioIncomeEdit.IsChecked = true;
        } else
		{
			RadioExpenseEdit.IsChecked = true;
		}

		EntryNameEdit.Text = transaction.Name;
		DatePickerEdit.Date = transaction.Date.Date;
		EntryValueEdit.Text = transaction.Value.ToString();
    }

    private async void TapGestureRecognizer_Edit(object sender, TappedEventArgs e)
    {
        KeyboardBugs.HideKeyboard();
        await Navigation.PopModalAsync();
    }

    private async void BtnSaveEdit_Clicked(object sender, EventArgs e)
    {
        if (isValidData() == false)
            return;

        SaveTransactionInDatabase();

        KeyboardBugs.HideKeyboard();

        await Navigation.PopModalAsync();
        WeakReferenceMessenger.Default.Send<string>("Registro atualizado no DB");
    }

    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction()
        {
            Type = RadioIncomeEdit.IsChecked ? TransactionType.Income : TransactionType.Expenses,
            Name = EntryNameEdit.Text,
            Date = DatePickerEdit.Date,
            Value = Math.Abs(double.Parse(EntryValueEdit.Text)),
        };

        _repository.Add(transaction);
    }

    private bool isValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        if (string.IsNullOrEmpty(EntryNameEdit.Text) || string.IsNullOrWhiteSpace(EntryNameEdit.Text))
        {
            sb.AppendLine("O campo 'Nome' deve ser preenchido");
            valid = false;
        }

        if (string.IsNullOrEmpty(EntryValueEdit.Text) || string.IsNullOrWhiteSpace(EntryValueEdit.Text))
        {
            sb.AppendLine("O campo 'Valor' deve ser preenchido");
            valid = false;
        }

        double result;
        if (!string.IsNullOrEmpty(EntryValueEdit.Text) && !double.TryParse(EntryValueEdit.Text, out result))
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