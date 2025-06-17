using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PersonalFinance.API.Data;

namespace PersonalFinance.API.Repositories
{
    // Lớp Repository cài đặt giao diện IRepository<T> cho mọi entity T.
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _ctx;
        private readonly DbSet<T> _dbSet;

        // Constructor duy nhất nhận ApplicationDbContext và khởi tạo _ctx và _dbSet.
        public Repository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        }

        // Sử dụng _ctx thay cho _context
        public IQueryable<T> Query => _ctx.Set<T>();

        public async Task<T?> GetByIdAsync(int id)
        {
            // Sử dụng FindAsync để truy xuất entity theo khóa chính.
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Lấy tất cả các entity từ cơ sở dữ liệu.
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            // Truy vấn các entity theo điều kiện predicate.
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            // Thêm một entity mới vào DbSet.
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            // Cập nhật một entity đã có.
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            // Xóa bỏ một entity khỏi DbSet.
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            // Lưu các thay đổi vào cơ sở dữ liệu thông qua DbContext.
            await _ctx.SaveChangesAsync();
        }
    }
}