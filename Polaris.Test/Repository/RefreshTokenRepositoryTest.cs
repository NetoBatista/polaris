using Polaris.Domain.Constant;
using Polaris.Domain.Entity;
using Polaris.Repository;
using Polaris.Test.Util;

namespace Polaris.Test.Repository
{
    [TestClass]
    public class RefreshTokenRepositoryTest
    {
        private RefreshTokenRepository _repository = new(new());
        private PolarisContext _context = new();

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
            var result = await _repository.Create(Guid.NewGuid());
            Assert.IsNotNull(result?.Id);
        }

        [TestMethod("Should be able get")]
        public async Task Get()
        {
            var entity = await _repository.Create(Guid.NewGuid());
            var result = await _repository.Get(entity);
            Assert.IsNotNull(result?.Id);
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            var entity = await _repository.Create(Guid.NewGuid());
            await _repository.Remove(entity);
            var result = await _repository.Get(entity);
            Assert.IsNull(result?.Id);
        }
    }
}
