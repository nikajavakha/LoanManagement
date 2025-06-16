
## პროექტის არქიტექტურა

პროექტი აგებულია **Clean Architecture** პრინციპების მიხედვით და იყენებს **CQRS + MediatR** ნიმუშს:

```
LoanManagement/
├── LoanManagement.API/          # REST API 
├── LoanManagement.Application/  # ბიზნეს ლოგიკა (Commands/Queries)
├── LoanManagement.Domain/       # დომენის მოდელები
└── LoanManagement.Infrastructure/ # მონაცემთა ბაზა და გარე სერვისები
└── LoanManagement.Web/ # ფრონტენდის აპლიკაცია
```

##  პროექტის გაშვება

### წინაპირობები

1. **.NET Core 7 SDK**
2. **Docker** (RabbitMQ-სთვის)
3. **SQL Server** (LocalDB ან სრული ვერსია)

### 1: RabbitMQ-ს გაშვება

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

RabbitMQ Management UI: http://localhost:15672 (guest/guest)

### 2: კონფიგურაცია

`LoanManagement.Api/appsettings.json` ფაილში შეამოწმეთ კონექშენები:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LoanManagementDb;Trusted_Connection=true;"
  },
  "LoanApplicationQueue": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  }
}
```

### 3: პროექტების გაშვება

Visual Studio-თი ან JetBrains Rider-ით გაუშვით LoanManagement.Api და LoanManagement.Web IIS Express პროფაილით 

პორტები და URL-ები მითითებულია თითოეული პროექტის `launchSettings.json` ფაილში.

## სისტემაში შესვლა

### დეფოლტ დამამტკიცებელი
- **პირადი ნომერი**: `00000000001`
- **პაროლი**: `Approver123`
- **როლი**: Approver (დამამტკიცებელი)

### ახალი მომხმარებლის რეგისტრაცია
ჩვეულებრივი მომხმარებლების რეგისტრაცია შესაძლებელია UI-დან.
