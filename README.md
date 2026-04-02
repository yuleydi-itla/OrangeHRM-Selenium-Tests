# OrangeHRM - Pruebas Automatizadas con Selenium

Este proyecto contiene las pruebas automatizadas que desarrollé usando 
Selenium WebDriver en C# sobre la aplicación OrangeHRM. 
## ¿Qué tecnologías usé?
- C# / .NET 8
- Selenium WebDriver
- NUnit
- ExtentReports
- Microsoft Edge

## ¿Sobre qué aplicación hice las pruebas?
Usé la demo pública de OrangeHRM, un sistema de gestión de recursos humanos.

🔗 https://opensource-demo.orangehrmlive.com
- Usuario: Admin
- Contraseña: admin123

## ¿Qué automaticé?
Desarrollé 15 casos de prueba distribuidos en 5 historias de usuario:

- HU-01: Inicio de sesión en el sistema
- HU-02: Registro de nuevo empleado
- HU-03: Búsqueda de empleados
- HU-04: Edición de datos de empleado
- HU-05: Eliminación de empleados

Cada historia tiene 3 tipos de prueba:
- Camino feliz
- Prueba negativa
- Prueba de límites

## ¿Cómo ejecutar el proyecto?
1. Clonar este repositorio
2. Abrir en Visual Studio 2022
3. Restaurar los paquetes NuGet
4. Colocar el archivo msedgedriver.exe en C:\WebDrivers\
5. Abrir el Test Explorer y ejecutar todas las pruebas

## Reporte
Al ejecutar las pruebas se genera automáticamente un reporte HTML 
con capturas de pantalla en:
bin/Debug/net8.0/Reports/TestReport.html

## Autora
**Yuleydi De Los Santos**  
Estudiante de Desarrollo de Software  
Instituto Tecnológico de Las Américas (ITLA)
