using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Entity;
using Polaris.Repository;
using Polaris.Test.Util;

namespace Polaris.Test.Repository
{
    [TestClass]
    public class ApplicationRepositoryTest
    {
        private ApplicationRepository _repository = new(new());
        private PolarisContext _context = new();

        [TestInitialize]
        public void Setup()
        {
            _context = DatabaseUtil.Create();
            _repository = new ApplicationRepository(_context);
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Dispose();
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var entity = new Application
            {
                Name = Guid.NewGuid().ToString()
            };
            entity = await _repository.Create(entity);
            var exists = await _context.Application.AnyAsync(x => x.Id == entity!.Id);
            Assert.IsTrue(exists);
        }

        [TestMethod("Should be able update")]
        public async Task Update()
        {
            var newName = Guid.NewGuid().ToString();
            var entity = await CreateMockApplication();

            entity.Name = newName;
            entity = await _repository.Update(entity);

            var exists = await _context.Application.AnyAsync(x => x.Id == entity!.Id);
            Assert.IsTrue(exists);
        }

        [TestMethod("Should not be able update")]
        public async Task NotUpdate()
        {
            var entity = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _repository.Update(entity));
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            var entity = await CreateMockApplication();
            var removed = await _repository.Remove(entity);
            var exists = await _context.Application.AnyAsync(x => x.Id == entity.Id);
            Assert.AreEqual(removed, !exists);
        }

        [TestMethod("Should not be able remove")]
        public async Task NotRemove()
        {
            var entity = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _repository.Remove(entity));
        }

        [TestMethod("Should be able get all")]
        public async Task GetAll()
        {
            var countEntities = 2;
            for (var index = 0; index < countEntities; index++)
            {
                await CreateMockApplication();
            }
            var entities = await _repository.GetAll();
            Assert.AreEqual(entities.Count, 2);
        }

        [TestMethod("Should be able already created")]
        public async Task AlreadyCreated()
        {
            var entity = await CreateMockApplication();
            entity.Id = Guid.NewGuid();
            var alreadyCreated = await _repository.Exists(entity);
            Assert.IsTrue(alreadyCreated);
        }

        [TestMethod("Should be able exists")]
        public async Task Exists()
        {
            var entity = await CreateMockApplication();
            var exists = await _repository.Exists(entity);
            Assert.IsTrue(exists);
        }

        [TestMethod("Should be able verify if has any member")]
        public async Task AnyMember()
        {
            var entity = await CreateMockApplication();
            var exists = await _repository.AnyMember(entity);
            Assert.IsTrue(exists);
        }

        private async Task<Application> CreateMockApplication()
        {
            var entity = new Application
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                MemberNavigation = new List<Member>
                {
                    new Member
                    {
                        UserId = Guid.NewGuid()
                    }
                }
            };
            _context.Application.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
