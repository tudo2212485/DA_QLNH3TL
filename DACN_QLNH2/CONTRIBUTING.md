# 🤝 Contributing to Restaurant Management System

Cảm ơn bạn đã quan tâm đến việc đóng góp cho dự án! Tài liệu này cung cấp hướng dẫn về quy trình đóng góp.

---

## 📋 Mục lục

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Coding Standards](#coding-standards)
- [Commit Messages](#commit-messages)
- [Pull Request Process](#pull-request-process)
- [Testing Guidelines](#testing-guidelines)

---

## Code of Conduct

Dự án này tuân thủ Contributor Covenant Code of Conduct. Khi tham gia, bạn cam kết:

- Tôn trọng tất cả mọi người
- Chấp nhận phản hồi mang tính xây dựng
- Tập trung vào điều tốt nhất cho cộng đồng

---

## Getting Started

### Prerequisites

- .NET 9 SDK
- Node.js 18+
- Git
- Visual Studio Code (hoặc IDE khác)

### Fork & Clone

```bash
# Fork repo trên GitHub
# Clone fork của bạn
git clone https://github.com/YOUR_USERNAME/DA_QLNH3TL.git
cd DA_QLNH3TL/DACN_QLNH2/QLNHWebApp

# Add upstream remote
git remote add upstream https://github.com/tudo2212485/DA_QLNH3TL.git
```

### Setup Local Environment

```bash
# Restore packages
dotnet restore

# Run database migrations
dotnet ef database update

# Run application
dotnet run
```

---

## Development Process

### 1. Create a Branch

```bash
# Update main branch
git checkout main
git pull upstream main

# Create feature branch
git checkout -b feature/your-feature-name
```

### Branch Naming Convention

| Type | Example |
|------|---------|
| Feature | `feature/add-customer-export` |
| Bug fix | `bugfix/fix-login-redirect` |
| Hotfix | `hotfix/critical-security-patch` |
| Refactor | `refactor/improve-order-service` |
| Docs | `docs/update-readme` |

### 2. Make Changes

- Write clean, readable code
- Follow coding standards (see below)
- Add comments for complex logic
- Update documentation if needed

### 3. Test Your Changes

```bash
# Run application
dotnet run

# Test manually
# Navigate to http://localhost:5000

# Test API with Swagger
# Navigate to http://localhost:5000/swagger
```

---

## Coding Standards

### C# / ASP.NET Core

#### Naming Conventions

```csharp
// ✅ GOOD
public class OrderManagementController : Controller
{
    private readonly RestaurantDbContext _context;
    private readonly ILogger<OrderManagementController> _logger;
    
    public async Task<IActionResult> GetOrders(int page = 1, int pageSize = 10)
    {
        var orders = await _context.Orders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return Json(orders);
    }
}

// ❌ BAD
public class orderController : Controller
{
    public RestaurantDbContext context;
    
    public IActionResult get_orders(int Page, int PageSize)
    {
        var Orders = context.Orders.ToList();
        return Json(Orders);
    }
}
```

#### Best Practices

- **Use async/await** for database operations
- **Use dependency injection** for services
- **Use LINQ** for queries
- **Handle exceptions** properly
- **Validate input** (server-side & client-side)
- **Use meaningful variable names**

```csharp
// ✅ GOOD
public async Task<IActionResult> Create(MenuItem menuItem, IFormFile? imageFile)
{
    if (!ModelState.IsValid)
    {
        return View(menuItem);
    }
    
    try
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "images", "menu");
            Directory.CreateDirectory(uploadsPath);
            
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            
            menuItem.ImageUrl = $"/images/menu/{fileName}";
        }
        
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Món ăn đã được thêm thành công!";
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating menu item");
        TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm món ăn.";
        return View(menuItem);
    }
}
```

### TypeScript / React

#### Component Structure

```typescript
// ✅ GOOD
interface PaymentPageProps {
    onPageChange: (page: string) => void;
}

export const PaymentPage: React.FC<PaymentPageProps> = ({ onPageChange }) => {
    const [order, setOrder] = useState<Order | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    
    useEffect(() => {
        const currentOrder = localStorage.getItem('current_order');
        if (currentOrder) {
            setOrder(JSON.parse(currentOrder));
        } else {
            toast.error('Không tìm thấy đơn hàng.');
            onPageChange('home');
        }
    }, [onPageChange]);
    
    const handlePayment = async () => {
        if (!order) return;
        
        setIsProcessing(true);
        try {
            await processPayment(order);
            toast.success('Thanh toán thành công!');
            onPageChange('home');
        } catch (error) {
            toast.error('Có lỗi xảy ra.');
        } finally {
            setIsProcessing(false);
        }
    };
    
    return <div>...</div>;
};

// ❌ BAD
export const paymentpage = (props) => {
    const [data, setdata] = useState(null);
    
    function click() {
        // process payment
    }
    
    return <div onClick={click}>...</div>;
};
```

### Razor Views

```cshtml
<!-- ✅ GOOD -->
@model QLNHWebApp.Models.Order

<div class="payment-page">
    <h1>@ViewData["Title"]</h1>
    
    @if (Model.OrderItems.Any())
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Món ăn</th>
                    <th>Số lượng</th>
                    <th>Giá</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in Model.OrderItems)
                {
                    <tr>
                        <td>@item.MenuItem?.Name</td>
                        <td>x @item.Quantity</td>
                        <td>@item.Price.ToString("N0") đ</td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

<!-- ❌ BAD -->
<div>
    @{
        var items = Model.OrderItems;
    }
    @foreach (var x in items)
    {
        <p>@x.MenuItem.Name - x@x.Quantity - @x.Price</p>
    }
</div>
```

---

## Commit Messages

### Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

| Type | Description | Example |
|------|-------------|---------|
| `feat` | New feature | `feat(order): add pagination to order list` |
| `fix` | Bug fix | `fix(payment): resolve quantity display bug` |
| `docs` | Documentation | `docs(readme): add installation guide` |
| `style` | Code formatting | `style(controller): format code` |
| `refactor` | Code refactoring | `refactor(service): improve data seeder` |
| `test` | Add/update tests | `test(api): add order API tests` |
| `chore` | Build/deps | `chore(deps): update EF Core to 9.0.1` |

### Examples

```bash
# Good commit messages
feat(menu): add drag and drop image upload
fix(auth): fix logout redirect to login page
docs(api): add Swagger documentation examples
refactor(dashboard): improve chart data loading
test(order): add unit tests for order service
chore(docker): update Dockerfile configuration

# Bad commit messages
update
fix bug
changes
wip
asdf
```

### Commit Guidelines

- **Use imperative mood**: "add" not "added" or "adds"
- **First line max 72 characters**
- **Explain what and why, not how**
- **Reference issues**: `fix(order): resolve #123 - payment calculation error`

---

## Pull Request Process

### 1. Before Submitting

- [ ] Code compiles without errors
- [ ] Manual testing completed
- [ ] Code follows style guidelines
- [ ] Comments added for complex logic
- [ ] Documentation updated (if needed)
- [ ] No console.log or debug code
- [ ] Commits are clean and follow convention

### 2. Create Pull Request

**Title Format:**
```
[Type] Brief description (max 72 chars)
```

**Examples:**
```
[Feature] Add customer export to Excel functionality
[Fix] Resolve login redirect issue for non-admin users
[Docs] Update README with Docker instructions
```

**PR Description Template:**

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change
- [ ] Documentation update

## Changes Made
- Change 1
- Change 2
- Change 3

## Screenshots (if applicable)
[Add screenshots here]

## Testing
How to test these changes:
1. Step 1
2. Step 2
3. Step 3

## Related Issues
Closes #123
```

### 3. Review Process

- Maintainers will review your PR
- Address feedback if requested
- Once approved, PR will be merged

---

## Testing Guidelines

### Manual Testing Checklist

#### For Backend Changes:

- [ ] API endpoints return correct status codes
- [ ] API responses match expected schema
- [ ] Error handling works properly
- [ ] Authorization rules are enforced
- [ ] Database queries are optimized
- [ ] No SQL injection vulnerabilities

#### For Frontend Changes:

- [ ] UI renders correctly on desktop
- [ ] UI renders correctly on mobile
- [ ] Forms validate properly
- [ ] Error messages display correctly
- [ ] Success messages display correctly
- [ ] No console errors

#### For Full-Stack Features:

- [ ] End-to-end flow works
- [ ] Data persists correctly
- [ ] Redirects work properly
- [ ] Authentication/Authorization enforced
- [ ] No memory leaks

### Testing Tools

```bash
# Test API with Swagger
http://localhost:5000/swagger

# Test with curl
curl -X GET "http://localhost:5000/api/Orders?page=1&pageSize=10"

# Test authentication
curl -X POST "http://localhost:5000/Auth/Login" \
  -d "username=admin&password=admin123"
```

---

## File Structure Guidelines

### New Controllers

Place in:
- MVC Controllers: `/Controllers/`
- API Controllers: `/Controllers/Api/`

### New Views

Place in: `/Views/{ControllerName}/`

### New Services

Place in: `/Services/`

### New Models

Add to: `/Models/RestaurantModels.cs` (or create new file if needed)

---

## Questions?

- **Issues:** https://github.com/tudo2212485/DA_QLNH3TL/issues
- **Discussions:** https://github.com/tudo2212485/DA_QLNH3TL/discussions
- **Email:** [your-email@example.com]

---

**Thank you for contributing! 🎉**

