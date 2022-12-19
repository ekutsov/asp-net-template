# **Проект Infrastructure (Data Access Library)**

## **Структура папок**

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

### **GlobalUsings.cs**
Добавляем все usings в этот файл