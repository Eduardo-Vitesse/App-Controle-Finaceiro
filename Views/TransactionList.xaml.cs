using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;

namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
    private ITransactionRepository _repository;

    public TransactionList(ITransactionRepository repository)
	{
        _repository = repository;

        InitializeComponent(); // Construção da tela para o usuário
        Reload();

        WeakReferenceMessenger.Default.Register<string>(this, (obj, msg) =>
        {
            Reload();
        });
	}

    private void Reload()
    {
        var items = _repository.GetAll();
        CollectionViewTransaction.ItemsSource = items;

        double income = items.Where(a => a.Type == Models.TransactionType.Income).Sum(a => a.Value);
        double expense = items.Where(a => a.Type == Models.TransactionType.Expenses).Sum(a => a.Value);
        double balance = income - expense;

        LabelIncome.Text = income.ToString("C");
        LabelExpense.Text = expense.ToString("C");
        LabelBalance.Text = balance.ToString("C");
    }

	private async void OnButtonClicked_To_TransactionAdd(object sender, EventArgs e)
	{
        var transactionAdd = Handler.MauiContext.Services.GetService<TransactionAdd>();
		await Navigation.PushModalAsync(transactionAdd);
	}

    private async void TapGestureRecognizer_EditTransaction(object sender, TappedEventArgs e)
    {
        var grid = (Grid)sender;
        var gesture = (TapGestureRecognizer)grid.GestureRecognizers[0];
        Transaction transaction = (Transaction)gesture.CommandParameter;

        var transactionEdit = Handler.MauiContext.Services.GetService<TransactionEdit>();
        transactionEdit.SetTransactionToEdit(transaction);
        await Navigation.PushModalAsync(transactionEdit);
    }

    private async void DeleteTransaction(object sender, TappedEventArgs e)
    {
        await AnimationBorder((Border)sender, true);
        bool result = await App.Current.MainPage.DisplayAlert("Excluir", "Tem certeza que deseja exluir?", "Sim", "Não");

        if (result == true)
        {
            Transaction transaction = (Transaction)e.Parameter;
            _repository.Delete(transaction);

            Reload();
        } else
        {
            await AnimationBorder((Border)sender, false);
            return;
        }
    }

    private Color _boderOrigialBackgroundColor;
    private string _labelOriginalText;

    private async Task AnimationBorder(Border border, bool isDeleteAnimation)
    {
        var label = (Label)border.Content;
        uint time = 500;

        if (isDeleteAnimation)
        {
            _boderOrigialBackgroundColor = border.BackgroundColor;
            _labelOriginalText = label.Text;

            await border.RotateYTo(90, time);
            border.BackgroundColor = Colors.Red;
            label.TextColor = Colors.White;
            label.Text = "X";
            await border.RotateYTo(180, time);
        } else
        {
            await border.RotateYTo(90, time);
            border.BackgroundColor = _boderOrigialBackgroundColor;
            label.TextColor = Colors.Black;
            label.Text = _labelOriginalText;
            await border.RotateYTo(0, time);
        }
    }
}