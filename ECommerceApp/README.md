# ECommerceApp — ASP.NET Core Web API + Blazor WebAssembly

A full-stack e-commerce project: Products, Cart, Orders, JWT Authentication, Role-based Authorization (Admin/Customer).
100% C# — no JavaScript required.

---

## 🇬🇧 How to Run (English)

### Prerequisites
1. Install [.NET 8 SDK](https://dotnet.microsoft.com/download) — check with `dotnet --version`
2. (Optional) Install [Visual Studio 2022](https://visualstudio.microsoft.com/) or use VS Code with the C# Dev Kit extension.

### Step 1 — Run the Backend API
```bash
cd Backend/ECommerceAPI
dotnet restore
dotnet ef migrations add InitialCreate   # (install tool first: dotnet tool install --global dotnet-ef)
dotnet run
```
The API will start at `https://localhost:7001` (check the console output — the port may differ; if it does, update it in `Frontend/ECommerceBlazor/Program.cs`).
Open `https://localhost:7001/swagger` to see and test all endpoints (Swagger UI).

### Step 2 — Run the Frontend (Blazor)
Open a **new terminal**:
```bash
cd Frontend/ECommerceBlazor
dotnet restore
dotnet run
```
Open the URL shown in the console (usually `https://localhost:7100` or similar).

### Step 3 — Try it out
1. Register a new account on the `/register` page.
2. Browse products on the homepage, add to cart.
3. Go to `/cart`, enter a shipping address, and checkout.
4. To test Admin features (create/edit/delete products), manually assign the "Admin" role to your user in the database, or extend the Register endpoint temporarily.

### Project Structure
```
Backend/ECommerceAPI/
  Models/        -> Product, Cart, Order, ApplicationUser
  Data/          -> ApplicationDbContext (EF Core)
  DTOs/          -> Request/response shapes (never expose EF models directly!)
  Controllers/   -> Auth, Products, Cart, Orders
  Services/      -> TokenService (JWT creation)

Frontend/ECommerceBlazor/
  Pages/         -> Products, Login, Register, Cart, Orders
  Services/      -> ApiService (HTTP calls), AuthStateService (login state)
  Layout/        -> MainLayout, NavMenu
```

### Next Features to Add (great for interviews)
- SignalR for real-time order status updates
- Image upload for products (IFormFile)
- Redis caching for the product list
- Unit tests with xUnit + Moq
- Docker + docker-compose for the whole stack
- Pagination UI + sorting on the frontend

---

## 🇱🇰 මෙහෙම Run කරගන්න (Sinhala)

### මුලින්ම ඕන දේවල්
1. [.NET 8 SDK](https://dotnet.microsoft.com/download) එක install කරගන්න — `dotnet --version` කියලා check කරන්න.
2. (Optional) Visual Studio 2022 හෝ VS Code + C# Dev Kit extension පාවිච්චි කරන්න.

### පියවර 1 — Backend API එක Run කරන්න
```bash
cd Backend/ECommerceAPI
dotnet restore
dotnet ef migrations add InitialCreate
dotnet run
```
API එක `https://localhost:7001` වගේ port එකක start වෙනවා (console එකේ පේනවා exact port එක). Swagger UI එකෙන් (`https://localhost:7001/swagger`) සියලුම endpoints test කරන්න පුළුවන්.

### පියවර 2 — Frontend (Blazor) එක Run කරන්න
**අලුත් terminal** එකක් open කරලා:
```bash
cd Frontend/ECommerceBlazor
dotnet restore
dotnet run
```

### පියවර 3 — App එක Test කරන්න
1. `/register` page එකෙන් account එකක් හදාගන්න.
2. Homepage එකෙන් products browse කරලා cart එකට add කරන්න.
3. `/cart` page එකට ගිහින් shipping address දාලා checkout කරන්න.
4. Admin features test කරන්න ඕන නම්, ඔයාගේ user account එකට database එකේ "Admin" role එක manual එකෙන් assign කරගන්න.

### Interview එකේදී කියන්න පුළුවන් Next Features
- SignalR (real-time updates)
- Product image upload
- Redis caching
- xUnit unit tests
- Docker containerization

---

## Key C# / .NET Concepts This Project Demonstrates
- **ASP.NET Core Web API** — Controllers, Routing, Middleware pipeline
- **Entity Framework Core** — Code-first models, migrations, LINQ queries
- **ASP.NET Core Identity** — User management, password hashing
- **JWT Authentication & Authorization** — `[Authorize]`, `[Authorize(Roles = "Admin")]`
- **Dependency Injection** — built into every Controller/Service constructor
- **DTO Pattern** — never exposing your database models directly over the API
- **Async/Await** — every database call is asynchronous
- **Blazor WebAssembly** — Components, data binding, forms, HttpClient
