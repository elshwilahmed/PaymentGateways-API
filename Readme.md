# Payment Gateway Integration Microservice 

A robust, scalable payment gateway integration microservice built with **.NET 9.0**. This API handles the complete checkout process and automates order status updates in real-time via Webhooks.

## Demo Video
>

https://github.com/user-attachments/assets/a1cf6614-47a2-43f2-8bfd-4cb4b152730c

## Key Features
- **Real-Time Webhook Processing:** Automatically listens to gateway callbacks to update order statuses (e.g., Pending ➡️ Paid) without manual intervention.
- **Scalable Architecture:** Designed to easily integrate multiple payment providers (currently integrated with **Paymob**) using the Factory Design Pattern.
- **Secure Transactions:** Generates payment intent URLs and seamlessly handles the redirect and callback flow securely.

## Tech Stack & Tools
- **Framework:** .NET 9.0 / ASP.NET Core Web API
- **Database:** Microsoft SQL Server & Entity Framework Core
- **Design Patterns:** Factory Pattern, Dependency Injection
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
- **Tools:** Swagger (OpenAPI), ngrok (for Webhook tunneling)

## Architecture Highlights
### Clean Architecture
The project is structured into independent layers to isolate business logic from external frameworks:
- `Domain`: Contains core entities like `Order` and fundamental business rules.
- `Application`: Contains DTOs, interfaces, and application-specific business logic.
- `Infrastructure`: Handles database context, EF Core migrations, and external API calls to payment gateways.
- `API`: The entry point with Controllers and dependency injection configurations.

### Factory Design Pattern
Implemented a `PaymentGatewayFactory` to dynamically resolve the appropriate payment gateway strategy at runtime. This adheres to the **Open/Closed Principle (SOLID)**, allowing the addition of new gateways (like PayPal or Fawry) in the future without modifying the core checkout logic.

## Local Setup & Running
### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/)
- SQL Server
- [ngrok](https://ngrok.com/) (for testing Webhooks locally)

### Steps
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/yourusername/PaymentGateway.IntegrationAPI.git](https://github.com/yourusername/PaymentGateway.IntegrationAPI.git)
   ```
2. **Update Connection String:** 
   Configure your SQL Server connection string in `appsettings.json`.

3. **Apply Migrations:**
   Run the following command in the Package Manager Console:
   ```bash
   Update-Database
   ```
4. **Run the API:**
   Start the project in Visual Studio. The API will run locally (e.g., on port 5175).

5. **Setup ngrok for Webhooks:**
   Run the following command to expose your local API so the payment gateway can send webhook requests to your localhost:
   ```bash
   ngrok http --domain=<YOUR_DOMAIN> http://localhost:<PORT> 
   ```
