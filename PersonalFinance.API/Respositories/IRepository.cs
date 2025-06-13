using System.Linq.Expressions;

namespace PersonalFinance.API.Repositories
{
    //định nghĩa các phương thức truy cập dữ liệu chung cho mọi entity
    public interface IRepository<T> where T : class
    {
        // Lấy entity theo id (không đồng bộ).
        Task<T?> GetByIdAsync(int id);

        // Lấy danh sách tất cả các entity.
        Task<IEnumerable<T>> GetAllAsync();
        // Lấy các entity thỏa mãn điều kiện (predicate) truyền vào
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Thêm một entity mới vào data store
        Task AddAsync(T entity);
        // Cập nhật thông tin của một entity
        void Update(T entity);

        // Xóa bỏ một entity
        void Remove(T entity);

        // Lưu các thay đổi lên cơ sở dữ liệu
        Task SaveChangesAsync();
    }
}