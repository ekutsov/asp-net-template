# **EKUTSOV Base Web API Template**

## **Установка  и создание шаблона**

### **Установка шаблона**
`dotnet new install EKUTSOV.WebAPI`

### **Создание шаблона**

`dotnet new ekutsov_web_api`<br>

**Параметры:** <br>
`--name` - Имя решения  

`--authorization` - Тип авторизации в приложении, на данный момент 2 типа: **NoAuth** и  **JWT**, по умолчанию **NoAuth**

`--includeDocker` - Добавить docker в приложение, по умолчанию **false**

`--disableTests` - Убрать тесты из приложения, по умолчанию **false**

## **Информация о проектах**

<details>
<summary><b style="font-size: 18px">Проект Domain</b></summary>

### **Constants** 
Константы проекта

### **DTO** 
DTO проекта

### **Enums** 
Enums проекта

### **Exceptions** 
Кастомные exceptions

### **ViewModels** 
View models проекта

</details>

<details>
<summary><b style="font-size: 18px">Проект Infrastructure</b></summary>
<br>

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
</details>


<details>
<summary><b style="font-size: 18px">Проект Core</b></summary>

### **Configuration**
- **AutomapperExtensions** - расшерения  automapper

### **Services**
- **Implementation** - сервисы проекта
- **Interfaces** - интерфейсы сервисов

</details>


<details>
<summary><b style="font-size: 18px">Проект API</b></summary>

### **Configuration**
- **IServiceCollectionExtensions** - весь DI проекта
### **Controllers**
Контроллеры проекта
### **Middlewares**
Кастомные middlewares
</details>



