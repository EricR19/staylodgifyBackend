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

### Autenticación con HttpOnly Cookies (Segura)

El sistema utiliza **HttpOnly Cookies** para autenticación, lo que proporciona las siguientes ventajas:

1. **Protección contra XSS**: Los tokens NO son accesibles desde JavaScript
2. **Envío automático**: Las cookies se envían automáticamente en cada request
3. **Control del servidor**: El backend controla completamente la sesión

#### Flujo de Autenticación

```
┌─────────────┐      POST /api/Auth/login        ┌─────────────┐
│   Frontend  │ ─────────────────────────────────>│   Backend   │
│  (Next.js)  │     { email, password }          │   (.NET)    │
└─────────────┘                                   └─────────────┘
                                                         │
                                                         ▼
                                                 ┌─────────────┐
                                                 │ Validate    │
                                                 │ Credentials │
                                                 └─────────────┘
                                                         │
                                                         ▼
┌─────────────┐      Set-Cookie: jwt=xxx;         ┌─────────────┐
│   Frontend  │ <─────────────────────────────────│   Backend   │
│             │      HttpOnly; Secure             │             │
└─────────────┘      + { user, tenant } JSON      └─────────────┘
```

#### Cambios requeridos en el Frontend (Next.js)

**ANTES (Inseguro):**
```typescript
// ❌ NO HACER ESTO - Token expuesto a XSS
const response = await fetch('/api/login', { ... });
const { token } = await response.json();
localStorage.setItem('token', token); // ❌ VULNERABLE

// ❌ NO HACER ESTO - Header manual
fetch('/api/data', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

**DESPUÉS (Seguro):**
```typescript
// ✅ CORRECTO - Cookie se maneja automáticamente
const response = await fetch('https://api.staylodgify.com/api/Auth/login', {
  method: 'POST',
  credentials: 'include', // ✅ CRÍTICO: Envía y recibe cookies
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});

const { success, user, tenant } = await response.json();
// ✅ El token está en la cookie HttpOnly, NO en la respuesta

// ✅ CORRECTO - Requests autenticados automáticamente
fetch('https://api.staylodgify.com/api/Propierties', {
  credentials: 'include' // ✅ La cookie se envía automáticamente
});
```

### Configuración de Fetch para Next.js

```typescript
// services/api.ts
class ApiService {
  private baseUrl = process.env.NEXT_PUBLIC_API_URL;

  async fetchWithAuth<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      ...options,
      credentials: 'include', // ✅ SIEMPRE incluir esto
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    });

    if (response.status === 401) {
      // Redirigir a login
      window.location.href = '/login';
      throw new Error('Session expired');
    }

    return response.json();
  }

  async login(email: string, password: string) {
    return this.fetchWithAuth('/api/Auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async logout() {
    return this.fetchWithAuth('/api/Auth/logout', { method: 'POST' });
  }

  async getCurrentUser() {
    return this.fetchWithAuth('/api/Auth/me');
  }
}
```

### Multi-tenant Security

- **Aislamiento total de datos** por tenant
- **Validación en middleware** de cada request autenticado
- **tenant_id embebido en JWT** - No se puede modificar desde el cliente
- **Rechazo automático** de intentos de acceso cross-tenant

### Middleware de Validación de Tenant

El sistema incluye un middleware que:
1. ✅ Valida que el token tenga tenant_id
2. ✅ Rechaza requests si el tenant está suspendido
3. ✅ Bloquea intentos de bypass vía query params o headers
4. ✅ Registra intentos de acceso no autorizados

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

⚠️ **IMPORTANTE**: El archivo `appsettings.json` contiene información sensible y NO está incluido en el repositorio por seguridad.

Copia el archivo de ejemplo y configúralo:

```bash
cp BookingSite.API/appsettings.json.example BookingSite.API/appsettings.json
```

Luego edita `BookingSite.API/appsettings.json` con tus configuraciones:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=bookingsite;User=root;Password=tu_password;"
  },
  "Jwt": {
    "Key": "GENERA_UNA_CLAVE_ALEATORIA_DE_AL_MENOS_32_CARACTERES",
    "Issuer": "BookingSiteAPI",
    "Audience": "BookingSiteClient",
    "ExpiresInMinutes": 60
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

**🔐 Notas de seguridad:**
- NUNCA compartas tu `appsettings.json` en repositorios públicos
- Genera una clave JWT única y segura (mínimo 32 caracteres)
- Cambia las credenciales de base de datos en producción
- En producción, usa variables de entorno en lugar de archivos de configuración

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

