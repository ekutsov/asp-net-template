# **A2SEVEN Base Web API Template**

## **Установка  и создание темплейта**

dotnet new "absolute path to project"

dotnet new a2seven_web_api
params:
--name Имя  решения  
--authorization Тип авторизации в приложении
--includeDocker Добавить docker в приложение

## **Проект Infrastructure (Data Access Library)**

### **Configuration**
Папка для конфигурации Entity с помощью FluentApi

**Пример реализации конфигурации**
```cs
public class EntityConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        /// TODO: Логика конфигурации Entity
    }
}
```

### **Context**
Папка для Database Context

### **Entites**
Сущности базы данных

### **GlobalUsings.cs**
Добавляем все usings в этот файл