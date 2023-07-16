using AppControleFinanceiro.Models;
using LiteDB;

namespace AppControleFinanceiro.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly LiteDatabase _database;
        private readonly string collectionName = "transactions";
        public TransactionRepository(LiteDatabase database)
        {
            _database = database;
        }

        public List<Transaction> GetAll() 
        {
            return _database
                .GetCollection<Transaction>(collectionName)
                .Query()
                .OrderByDescending(a => a.Date)
                .ToList();
        }

        public void Add(Transaction transaction) 
        {
            var coll = _database.GetCollection<Transaction>(collectionName);
            coll.Insert(transaction);
            coll.EnsureIndex(a => a.Date);
        }

        public void Update(Transaction transaction)
        {
            var coll = _database.GetCollection<Transaction>(collectionName);
            coll.Update(transaction);
        }

        public void Delete(Transaction transaction) 
        {
            var coll = _database.GetCollection<Transaction>(collectionName);
            coll.Delete(transaction.Id);
        }
    }
}
