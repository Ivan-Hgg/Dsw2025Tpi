using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Repositories;

public class EfRepository : IRepository
{
    private readonly Dsw2025TpiContext _context;

    public EfRepository(Dsw2025TpiContext context)
    {
        _context = context;
    }

    public async Task<T> Add<T>(T entity) where T : EntityBase
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> Delete<T>(T entity) where T : EntityBase
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> First<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
    {
        return await ApplyIncludes(_context.Set<T>(), include).FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>?> GetAll<T>(params string[] include) where T : EntityBase
    {
        return await ApplyIncludes(_context.Set<T>(), include).ToListAsync();
    }

    public async Task<T?> GetById<T>(Guid id, params string[] include) where T : EntityBase
    {
        return await ApplyIncludes(_context.Set<T>(), include).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<T>?> GetFiltered<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
    {
        return await ApplyIncludes(_context.Set<T>(), include).Where(predicate).ToListAsync();
    }

    public async Task<T> Update<T>(T entity) where T : EntityBase
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    //Join
    private static IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, string[] includes) where T : EntityBase
    {
        if (includes == null || includes.Length == 0)
            return query;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query;
    }
}
