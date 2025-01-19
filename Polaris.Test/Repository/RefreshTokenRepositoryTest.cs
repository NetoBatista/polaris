using System;
using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Repository;
using Polaris.Test.Util;

namespace Polaris.Test.Repository;

[TestClass]
public class RefreshTokenRepositoryTest
{
    private RefreshTokenRepository _repository;
    private PolarisContext _context;

    [TestInitialize]
    public void Setup()
    {
        _context = DatabaseUtil.Create();
        _repository = new RefreshTokenRepository(_context);
    }

    [TestCleanup]
    public void Teardown()
    {
        _context.Dispose();
    }

    [TestMethod("Should be able create")]
    public async Task Create()
    {
        var entity = await _repository.Create(Guid.NewGuid());
        var exists = await _context.RefreshToken.AnyAsync(x => x.Id == entity!.Id);
        Assert.IsTrue(exists);
    }

    [TestMethod("Should be able get by id")]
    public async Task GetById()
    {
        var entity = await _repository.Create(Guid.NewGuid());
        var exists = await _repository.Get(entity!.Id);
        Assert.IsNotNull(exists);
    }
    
    [TestMethod("Should not be able get by id")]
    public async Task NotGetById()
    {
        var entity = await _repository.Create(Guid.NewGuid());
        await _repository.Update(entity.Id);
        var exists = await _repository.Get(entity!.Id);
        Assert.IsNull(exists);
    }

    [TestMethod("Should be able get by AuthenticationId")]
    public async Task GetByAuthenticationId()
    {
        var entity = await _repository.Create(Guid.NewGuid());
        var exists = await _repository.Get(entity!.AuthenticationId);
        Assert.IsNotNull(exists);
    }

    [TestMethod("Should be able Update")]
    public async Task Update()
    {
        var entity = await _repository.Create(Guid.NewGuid());
        var updated = await _repository.Update(entity!.Id);
        Assert.IsTrue(updated);
    }

    [TestMethod("Should not be able Update")]
    public async Task NotUpdate()
    {
        var updated = await _repository.Update(Guid.NewGuid());
        Assert.IsFalse(updated);
    }
}
