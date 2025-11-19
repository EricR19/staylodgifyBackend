# 🏨 StayLodgify Backend - BookingSite API

Sistema de gestión de reservas hoteleras multi-tenant desarrollado con .NET 8 y arquitectura limpia.

## 📋 Descripción

StayLodgify Backend es una API RESTful robusta diseñada para gestionar sistemas de reservas hoteleras con soporte multi-tenant. Permite a múltiples propiedades hoteleras gestionar sus reservas, huéspedes, pagos y disponibilidad de manera independiente y segura.

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture** organizado en 4 capas:

```
BookingSiteApi/
├── BookingSite.API/              # Capa de Presentación
│   ├── Controllers/              # Endpoints REST
│   └── Program.cs               # Configuración y DI
├── BookingSite.Application/      # Capa de Aplicación
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Services/                # Lógica de negocio
│   └── UseCases/                # Casos de uso
├── BookingSite.Domain/           # Capa de Dominio
│   ├── Entities/                # Entidades del negocio
│   └── Repositories/            # Interfaces de repositorios
└── BookingSite.Infrastructure/   # Capa de Infraestructura
    ├── Context/                 # DbContext de EF Core
    └── Repositories/            # Implementación de repositorios
```

## 🚀 Tecnologías

- **.NET 8.0** - Framework principal
- **Entity Framework Core 8.0** - ORM
- **MySQL** - Base de datos (Pomelo.EntityFrameworkCore.MySql)
- **JWT Bearer Authentication** - Autenticación y autorización
- **Swagger/OpenAPI** - Documentación de API
- **BCrypt.Net** - Encriptación de contraseñas

## 📊 Modelo de Datos

### Entidades Principales

- **Tenant** - Clientes multi-tenant con planes de suscripción
- **Property** - Propiedades/hoteles pertenecientes a tenants
- **Room** - Habitaciones dentro de las propiedades
- **Reservation** - Reservas realizadas por huéspedes
- **Guest** - Información de huéspedes
- **User** - Usuarios del sistema con roles
- **Payment** - Gestión de pagos
- **Receipt** - Comprobantes de pago
- **Availability** - Control de disponibilidad de habitaciones
- **Logs** - Registro de actividades del sistema

## 🔐 Seguridad

### Autenticación
- **JWT Bearer Tokens** con soporte dual:
  - Authorization header (Bearer token)
  - Cookie-based authentication
- Sistema de refresh tokens
- Reset de contraseñas con tokens temporales

### Multi-tenant
- Aislamiento de datos por tenant
- Validación automática de pertenencia de recursos
- Control de acceso basado en roles (RBAC)

## 🎯 Características Principales

### Gestión de Propiedades
- CRUD completo de propiedades hoteleras
- Gestión de habitaciones por propiedad
- Soporte para múltiples imágenes

### Sistema de Reservas
- Creación de reservas por huéspedes
- Validación de disponibilidad automática
- Estados de reserva (pending, confirmed, cancelled)
- Historial de reservas

### Gestión de Pagos
- Registro de pagos por reserva
- Múltiples métodos de pago
- Generación de recibos/comprobantes
- Upload de comprobantes de pago

### Panel de Administración
- Dashboard por tenant
- Gestión de usuarios y roles
- Logs de actividades
- Reportes y estadísticas

## 📡 API Endpoints

### Autenticación
```
POST   /api/Auth/login              # Iniciar sesión
POST   /api/Auth/logout             # Cerrar sesión
POST   /api/Auth/register           # Registrar usuario
POST   /api/Auth/change-password    # Cambiar contraseña
POST   /api/Auth/reset-password     # Solicitar reset de contraseña
```

### Propiedades
```
GET    /api/Propierties             # Listar propiedades
GET    /api/Propierties/{id}        # Obtener propiedad
POST   /api/Propierties             # Crear propiedad
PUT    /api/Propierties/{id}        # Actualizar propiedad
DELETE /api/Propierties/{id}        # Eliminar propiedad
```

### Habitaciones
```
GET    /api/Rooms                   # Listar habitaciones
GET    /api/Rooms/{id}              # Obtener habitación
POST   /api/Rooms                   # Crear habitación
PUT    /api/Rooms/{id}              # Actualizar habitación
DELETE /api/Rooms/{id}              # Eliminar habitación
```

### Reservas
```
GET    /api/Reservations            # Listar reservas
GET    /api/Reservations/{id}       # Obtener reserva
POST   /api/Reservations            # Crear reserva
PUT    /api/Reservations/{id}       # Actualizar reserva
DELETE /api/Reservations/{id}       # Eliminar reserva
```

### Disponibilidad
```
GET    /api/Availability            # Consultar disponibilidad
POST   /api/Availability/check      # Verificar disponibilidad
```

### Pagos y Recibos
```
GET    /api/Payments                # Listar pagos
POST   /api/Payments                # Registrar pago
GET    /api/Receipts                # Listar recibos
POST   /api/Receipts                # Crear recibo
```

### Tenants
```
GET    /api/Tenants                 # Listar tenants
GET    /api/Tenants/{id}            # Obtener tenant
POST   /api/Tenants                 # Crear tenant
PUT    /api/Tenants/{id}            # Actualizar tenant
```

## 🛠️ Configuración

### Prerrequisitos

- .NET 8.0 SDK o superior
- MySQL 8.0 o superior
- Un IDE (Visual Studio, Rider, VS Code)

### Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/EricR19/staylodgifyBackend.git
cd staylodgifyBackend
```

2. **Restaurar paquetes**
```bash
dotnet restore
```

3. **Configurar la base de datos**

Crea el archivo `appsettings.json` en `BookingSite.API/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=bookingsite;User=root;Password=tu_password;"
  },
  "Jwt": {
    "Key": "tu_clave_secreta_super_segura_de_al_menos_32_caracteres",
    "Issuer": "BookingSiteAPI",
    "Audience": "BookingSiteClient"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

4. **Aplicar migraciones** (si existen)
```bash
cd BookingSite.API
dotnet ef database update
```

5. **Ejecutar el proyecto**
```bash
dotnet run --project BookingSite.API
```

La API estará disponible en:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## 🧪 Testing

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura
dotnet test /p:CollectCoverage=true
```

## 📦 Estructura de DTOs

### Ejemplo: ReservationDto
```csharp
public class ReservationDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
}
```

## 🔄 CORS

El API está configurado para permitir requests desde:
- `http://localhost:3000`
- `http://localhost:3001`
- `http://localhost:3002`

Para modificar los orígenes permitidos, edita `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

## 📝 Convenciones de Código

- **Nombres de variables**: camelCase
- **Nombres de clases**: PascalCase
- **Nombres de interfaces**: IPascalCase
- **Propiedades de BD**: Snake_case (por convención de MySQL)
- **Async/Await**: Todos los métodos que acceden a BD deben ser asíncronos

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es privado y confidencial.

## 👥 Autor

**Eric Ruiz** - [GitHub](https://github.com/EricR19)

## 📞 Contacto

Para preguntas o soporte, contacta al equipo de desarrollo.

---

⭐ Si este proyecto te ha sido útil, considera darle una estrella en GitHub!

