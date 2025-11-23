#!/bin/bash
set -e

echo "🚀 Starting Render deployment..."

# Verificar que las variables de entorno estén configuradas
if [ -z "$ConnectionStrings__DefaultConnection" ]; then
    echo "⚠️  Warning: ConnectionStrings__DefaultConnection not set"
fi

if [ -z "$Jwt__Key" ]; then
    echo "⚠️  Warning: Jwt__Key not set"
fi

echo "✅ Environment variables check complete"

# Aplicar migraciones de base de datos
echo "📊 Applying database migrations..."
dotnet ef database update --project BookingSite.Infrastructure --startup-project BookingSite.API --no-build

echo "✅ Migrations applied successfully"
echo "🎉 Deployment complete!"

