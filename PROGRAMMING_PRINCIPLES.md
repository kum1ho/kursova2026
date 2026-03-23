### Принципи програмування, використані в проєкті

У даному проєкті були застосовані основні принципи програмування, які забезпечують зрозумілість, масштабованість та підтримуваність коду.

1. Принцип єдиної відповідальності (Single Responsibility Principle — SRP)
Кожен клас у проєкті виконує лише одну функцію.
Приклад:
Controllers відповідають лише за обробку HTTP-запитів
Services містять бізнес-логіку
Models описують структуру даних
 Controllers:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreAPI/Controllers
 Services:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreAPI/Services
Це дозволяє змінювати окремі частини системи незалежно одна від одної.

2. Розділення відповідальностей (Separation of Concerns — SoC)
Проєкт поділений на окремі логічні частини:
PetStoreAPI — бізнес-логіка та робота з даними
PetStoreDesktop — інтерфейс користувача
база даних (petstore_database.sql)
 PetStoreAPI:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreAPI
 PetStoreDesktop:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreDesktop
Це дозволяє розробляти та тестувати кожну частину незалежно.

3. Використання патерну MVVM
У десктопному додатку використовується архітектурний патерн MVVM (Model-View-ViewModel).
Структура:
Models — дані
Views — інтерфейс (XAML)
ViewModels — логіка взаємодії
 ViewModels:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreDesktop/ViewModels
 Views:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreDesktop/Views
Переваги:
- розділення логіки та UI
- краща тестованість
- зручність підтримки

4. Принцип DRY (Don't Repeat Yourself)
У проєкті уникнуто дублювання коду.
Приклад:
Бізнес-логіка винесена у Services, а не дублюється в Controllers
https://github.com/kum1ho/kursova2026/tree/master/PetStoreAPI/Services
Це спрощує внесення змін і зменшує кількість помилок.

5. Патерн Repository
Доступ до бази даних реалізований через окремий шар роботи з даними.
Контролери не взаємодіють напряму з базою даних, а використовують відповідні сервіси/репозиторії.
https://github.com/kum1ho/kursova2026/tree/master/PetStoreAPI
Переваги:
- інкапсуляція роботи з даними
- можливість зміни реалізації БД
- зручність тестування

6. Dependency Injection (DI)
У проєкті використовується механізм впровадження залежностей.
Приклад:
Services передаються у Controllers через конструктор
 Program.cs:
https://github.com/kum1ho/kursova2026/blob/master/PetStoreAPI/Program.cs
Переваги:
- зменшення зв’язності між компонентами
- гнучкість архітектури
- спрощення тестування

7. Валідація даних
Валідація використовується для перевірки коректності введених даних.
 Models:
https://github.com/kum1ho/kursova2026/tree/master/PetStoreAPI/Models
Це допомагає уникнути некоректних записів у базі даних.

8. Цілісність даних у базі даних
У базі даних використовуються обмеження (constraints) для забезпечення правильності даних.
Приклад:
У таблиці OrderDetails може бути заповнене або ProductID, або AnimalID, але не обидва одночасно
https://github.com/kum1ho/kursova2026/blob/master/petstore_database.sql
Це забезпечує логічну цілісність даних.

Висновок:
Використання наведених принципів дозволило створити структурований, зрозумілий та масштабований проєкт, який легко підтримувати та розширювати.
