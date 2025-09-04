﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using TrafficControlSystem.Context;
using TrafficControlSystem.Interface.Respositories;
using TrafficControlSystem.Contracts;
using Microsoft.EntityFrameworkCore;

namespace TrafficControlSystem.Implementation.Respositories;

public class BaseRepository<T> : IBaseRepo<T> where T : class, new()
{
    protected TrafficControlSystemContext context;
    public async Task<T> Create(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }
    public async Task<T> Update(T entity)
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
    public async Task<T> Get(Expression<Func<T, bool>> expression)
    {
        var ans = await context.Set<T>().FirstOrDefaultAsync(expression);
        return ans;
    }
    public async Task<IList<T>> GetAll()
    {
        return await context.Set<T>().ToListAsync();
    }
    public async Task<bool> Delete(T entity)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<IList<T>> GetByExpression(Expression<Func<T, bool>> expression)
    {
        var get = await context.Set<T>().Where(expression).ToListAsync();
        return get;
    }
}
