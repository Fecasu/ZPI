using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Mapping;

namespace WebStore.Services.ConcreteServices;

public abstract class BaseService(ApplicationDbContext context, StoreMapper mapper, ILogger logger)
{
    protected ApplicationDbContext Context { get; } = context;
    protected StoreMapper Mapper { get; } = mapper;
    protected ILogger Logger { get; } = logger;

    protected T Execute<T>(Func<T> operation)
    {
        try { return operation(); }
        catch (Exception e) { Logger.LogError(e, "Operacja usługi {Service} nie powiodła się", GetType().Name); throw; }
    }

    protected T Write<T>(Func<T> operation) => Execute(() =>
    {
        // Kontekst usługi jest granicą jednostki pracy: nie przyjmujemy cudzych niezapisanych zmian.
        if (Context.ChangeTracker.HasChanges())
            throw new InvalidOperationException("Przed wywołaniem usługi zapisz zmiany kontekstu.");
        using var transaction = Context.Database.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            var result = operation();
            transaction.Commit();
            Context.ChangeTracker.Clear();
            return result;
        }
        catch
        {
            transaction.Rollback();
            Context.ChangeTracker.Clear();
            throw;
        }
    });

    protected TVm? One<TEntity, TVm>(IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>> predicate, Func<TEntity, TVm> map) where TEntity : class where TVm : class
        => Execute(() =>
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var entity = query.AsNoTracking().FirstOrDefault(predicate);
            return entity is null ? null : map(entity);
        });

    protected IReadOnlyList<TVm> Many<TEntity, TVm>(IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? predicate, Func<TEntity, TVm> map) where TEntity : class
        => Execute(() => (predicate is null ? query : query.Where(predicate)).AsNoTracking().ToList().Select(map).ToArray());

    protected TUser RequiredUser<TUser>(int id) where TUser : User
    {
        var user = Context.Users.SingleOrDefault(x => x.Id == id)
            ?? throw new KeyNotFoundException($"Nie znaleziono użytkownika {id}.");
        return user as TUser ?? throw new ArgumentException($"Użytkownik {id} ma niewłaściwy typ.");
    }

    protected static void ValidId(int? id)
    {
        if (id < 0) throw new ArgumentException("ID nie może być ujemne.", nameof(id));
    }
    protected static void RequiredText(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Wymagana niepusta wartość.", name);
    }
}
