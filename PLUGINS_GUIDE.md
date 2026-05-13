# Guía de Extensibilidad: Plugins de Importación de Productos

Esta guía explica cómo desarrollar e integrar nuevos módulos de importación de productos en el sistema **DarkKitchen**. Gracias a nuestra arquitectura basada en *Reflection*, puedes agregar soporte para nuevos formatos de archivo (XML, CSV, etc.) simplemente agregando una librería (`.dll`) en la carpeta correspondiente.

## 1. Requisitos Técnicos
Para crear un nuevo importador, debes crear un proyecto de tipo **Class Library** en .NET 8 y referenciar el siguiente proyecto de nuestra solución:
- `DarkKitchen.Plugin.Contracts`: Contiene la interfaz y los DTOs necesarios.

## 2. Implementación
Debes crear una clase pública que implemente la interfaz `IProductImporter`.

### Interfaz `IProductImporter`
```csharp
public interface IProductImporter
{
    string Name { get; }
    IEnumerable<ProductImportDto> ImportProducts(string filePath);
}
```
- **`Name`**: Es el nombre amigable que aparecerá en el sistema (ej: "XML Importer").
- **`ImportProducts`**: Recibe la ruta del archivo y debe devolver una lista de DTOs.

### Estructura de Datos (DTOs)
El sistema espera objetos de tipo `ProductImportDto`:
- `Code`: String (5-20 caracteres).
- `Name`: String (10-50 caracteres).
- `Description`: String (20-500 caracteres).
- `LineName`: Nombre de la línea (se reutiliza si ya existe).
- `CategoryName`: Nombre de la categoría (se reutiliza si ya existe).
- `Price`: Decimal positivo.
- `Images`: Lista de `ImageImportDto` (cada una con `Url` y `SizeInBytes`).

## 3. Ejemplo de Implementación (XML)
```csharp
public class XmlProductImporter : IProductImporter
{
    public string Name => "XML Importer";

    public IEnumerable<ProductImportDto> ImportProducts(string filePath)
    {
        // 1. Leer el archivo XML desde filePath
        // 2. Mapear a objetos ProductImportDto
        // 3. Devolver la colección
    }
}
```

## 4. Despliegue e Integración
Una vez que tu librería esté compilada:
1. Genera el archivo `.dll` de tu proyecto.
2. Copia la DLL en la carpeta `/Plugins` ubicada en el directorio raíz de la API ejecutada.
3. El sistema detectará automáticamente el nuevo importador en el próximo inicio o mediante el endpoint `GET /api/plugins/importers`.

## 5. Consideraciones
- **Robustez**: El sistema central de DarkKitchen ya maneja validaciones de dominio y duplicados. Tu plugin debe enfocarse únicamente en la lectura y mapeo del formato de origen.
- **Transparencia**: No es necesario registrar tu clase en ningún contenedor de dependencias; el cargador dinámico se encarga de todo.
