-- Очистка таблиц (если нужно)
DELETE FROM Recipes;
DELETE FROM Appointments;
DELETE FROM MedicalCards;
DELETE FROM Patients;
DELETE FROM Doctors;
DELETE FROM DoctorTypes;
GO

-- 1. Добавляем типы врачей (5 записей)
INSERT INTO DoctorTypes (Id, Name, Description)
VALUES
    (NEWID(), N'Терапевт', N'Врач общей практики, первичный прием'),
    (NEWID(), N'Кардиолог', N'Специалист по заболеваниям сердечно-сосудистой системы'),
    (NEWID(), N'Невролог', N'Специалист по заболеваниям нервной системы'),
    (NEWID(), N'Хирург', N'Специалист по оперативным вмешательствам'),
    (NEWID(), N'Педиатр', N'Специалист по детским заболеваниям');
GO

-- 2. Добавляем врачей (6 записей, включая Дарину) - получаем ID типов в одном пакете
DECLARE @TherapistId UNIQUEIDENTIFIER, @CardiologistId UNIQUEIDENTIFIER, @NeurologistId UNIQUEIDENTIFIER, @SurgeonId UNIQUEIDENTIFIER, @PediatricianId UNIQUEIDENTIFIER;

SELECT @TherapistId = Id FROM DoctorTypes WHERE Name = N'Терапевт';
SELECT @CardiologistId = Id FROM DoctorTypes WHERE Name = N'Кардиолог';
SELECT @NeurologistId = Id FROM DoctorTypes WHERE Name = N'Невролог';
SELECT @SurgeonId = Id FROM DoctorTypes WHERE Name = N'Хирург';
SELECT @PediatricianId = Id FROM DoctorTypes WHERE Name = N'Педиатр';

INSERT INTO Doctors (Id, FullName, Phone, Email, Password, DoctorTypeId)
VALUES
    (NEWID(), N'Петров Иван Сергеевич', N'+7-911-123-45-67', N'petrov@medicare.ru', N'1', @TherapistId),
    (NEWID(), N'Сидорова Анна Михайловна', N'+7-912-234-56-78', N'sidorova@medicare.ru', N'2', @CardiologistId),
    (NEWID(), N'Козлов Дмитрий Владимирович', N'+7-913-345-67-89', N'kozlov@medicare.ru', N'3', @NeurologistId),
    (NEWID(), N'Николаева Елена Петровна', N'+7-914-456-78-90', N'nikolaeva@medicare.ru', N'4', @SurgeonId),
    (NEWID(), N'Васильев Алексей Игоревич', N'+7-915-567-89-01', N'vasilev@medicare.ru', N'5', @PediatricianId),
    (NEWID(), N'Дарина Смирнова', N'+7-916-678-90-12', N'darina@medicare.ru', N'5', @TherapistId);
GO

-- 3. Добавляем пациентов (10 записей - 5 общих и 5 для Дарины)
INSERT INTO Patients (Id, FullName, Phone, Email)
VALUES
    -- Общие пациенты
    (NEWID(), N'Иванов Сергей Александрович', N'+7-921-111-11-11', N'ivanov@mail.ru'),
    (NEWID(), N'Смирнова Ольга Викторовна', N'+7-922-222-22-22', N'smirnova@gmail.com'),
    (NEWID(), N'Кузнецов Михаил Петрович', N'+7-923-333-33-33', N'kuznetsov@yandex.ru'),
    (NEWID(), N'Попова Екатерина Сергеевна', N'+7-924-444-44-44', N'popova@mail.ru'),
    (NEWID(), N'Федоров Андрей Николаевич', N'+7-925-555-55-55', N'fedorov@gmail.com'),
    -- Пациенты Дарины
    (NEWID(), N'Орлова Мария Дмитриевна', N'+7-926-666-66-66', N'orlova@mail.ru'),
    (NEWID(), N'Громов Артем Сергеевич', N'+7-927-777-77-77', N'gromov@gmail.com'),
    (NEWID(), N'Воронцова Алина Игоревна', N'+7-928-888-88-88', N'vorontsova@yandex.ru'),
    (NEWID(), N'Титов Владислав Петрович', N'+7-929-999-99-99', N'titov@mail.ru'),
    (NEWID(), N'Крылова Юлия Александровна', N'+7-930-000-00-00', N'krylova@gmail.com');
GO

-- 4. Добавляем медицинские карты (10 записей) - получаем ID пациентов
DECLARE 
    @Patient1 UNIQUEIDENTIFIER, @Patient2 UNIQUEIDENTIFIER, @Patient3 UNIQUEIDENTIFIER, 
    @Patient4 UNIQUEIDENTIFIER, @Patient5 UNIQUEIDENTIFIER, @Patient6 UNIQUEIDENTIFIER,
    @Patient7 UNIQUEIDENTIFIER, @Patient8 UNIQUEIDENTIFIER, @Patient9 UNIQUEIDENTIFIER, 
    @Patient10 UNIQUEIDENTIFIER;

-- Получаем ID всех пациентов
SELECT @Patient1 = Id FROM Patients WHERE FullName = N'Иванов Сергей Александрович';
SELECT @Patient2 = Id FROM Patients WHERE FullName = N'Смирнова Ольга Викторовна';
SELECT @Patient3 = Id FROM Patients WHERE FullName = N'Кузнецов Михаил Петрович';
SELECT @Patient4 = Id FROM Patients WHERE FullName = N'Попова Екатерина Сергеевна';
SELECT @Patient5 = Id FROM Patients WHERE FullName = N'Федоров Андрей Николаевич';
SELECT @Patient6 = Id FROM Patients WHERE FullName = N'Орлова Мария Дмитриевна';
SELECT @Patient7 = Id FROM Patients WHERE FullName = N'Громов Артем Сергеевич';
SELECT @Patient8 = Id FROM Patients WHERE FullName = N'Воронцова Алина Игоревна';
SELECT @Patient9 = Id FROM Patients WHERE FullName = N'Титов Владислав Петрович';
SELECT @Patient10 = Id FROM Patients WHERE FullName = N'Крылова Юлия Александровна';

INSERT INTO MedicalCards (Id, MedicalHistory, Allergies, CurrentMedications, PatientId)
VALUES
    -- Медкарты общих пациентов
    (NEWID(), N'Гипертоническая болезнь I стадии', N'Пенициллин', N'Эналаприл 5 мг 1 раз в день', @Patient1),
    (NEWID(), N'ИБС, стенокардия напряжения', N'Нет', N'Аспирин 100 мг, Бисопролол 2.5 мг', @Patient2),
    (NEWID(), N'Остеохондроз шейного отдела', N'Йод', N'Диклофенак при болях', @Patient3),
    (NEWID(), N'Сахарный диабет 2 типа', N'Нет', N'Метформин 500 мг 2 раза в день', @Patient4),
    (NEWID(), N'Бронхиальная астма', N'Аспирин, пыльца', N'Сальбутамол по требованию', @Patient5),
    -- Медкарты пациентов Дарины
    (NEWID(), N'Гастрит в стадии ремиссии', N'Мед, орехи', N'Омепразол 20 мг утром', @Patient6),
    (NEWID(), N'Вегетососудистая дистония', N'Нет', N'Настойка пустырника при тревоге', @Patient7),
    (NEWID(), N'Хронический тонзиллит', N'Антибиотики цефалоспорины', N'Тонзилгон по 25 капель 3 раза', @Patient8),
    (NEWID(), N'Артериальная гипотензия', N'Нет', N'Элеутерококк 20 капель утром', @Patient9),
    (NEWID(), N'Мигрень', N'Шоколад, цитрусовые', N'Суматриптан при приступе', @Patient10);
GO

-- 5. Добавляем записи на прием (15 записей) - получаем ID врачей и пациентов
DECLARE 
    @Doctor1 UNIQUEIDENTIFIER, @Doctor2 UNIQUEIDENTIFIER, @Doctor3 UNIQUEIDENTIFIER, 
    @Doctor4 UNIQUEIDENTIFIER, @Doctor5 UNIQUEIDENTIFIER, @Doctor6 UNIQUEIDENTIFIER,
    @Patient1 UNIQUEIDENTIFIER, @Patient2 UNIQUEIDENTIFIER, @Patient3 UNIQUEIDENTIFIER, 
    @Patient4 UNIQUEIDENTIFIER, @Patient5 UNIQUEIDENTIFIER, @Patient6 UNIQUEIDENTIFIER,
    @Patient7 UNIQUEIDENTIFIER, @Patient8 UNIQUEIDENTIFIER, @Patient9 UNIQUEIDENTIFIER, 
    @Patient10 UNIQUEIDENTIFIER;

-- Получаем ID врачей
SELECT @Doctor1 = Id FROM Doctors WHERE FullName = N'Петров Иван Сергеевич';
SELECT @Doctor2 = Id FROM Doctors WHERE FullName = N'Сидорова Анна Михайловна';
SELECT @Doctor3 = Id FROM Doctors WHERE FullName = N'Козлов Дмитрий Владимирович';
SELECT @Doctor4 = Id FROM Doctors WHERE FullName = N'Николаева Елена Петровна';
SELECT @Doctor5 = Id FROM Doctors WHERE FullName = N'Васильев Алексей Игоревич';
SELECT @Doctor6 = Id FROM Doctors WHERE FullName = N'Дарина Смирнова';

-- Получаем ID пациентов
SELECT @Patient1 = Id FROM Patients WHERE FullName = N'Иванов Сергей Александрович';
SELECT @Patient2 = Id FROM Patients WHERE FullName = N'Смирнова Ольга Викторовна';
SELECT @Patient3 = Id FROM Patients WHERE FullName = N'Кузнецов Михаил Петрович';
SELECT @Patient4 = Id FROM Patients WHERE FullName = N'Попова Екатерина Сергеевна';
SELECT @Patient5 = Id FROM Patients WHERE FullName = N'Федоров Андрей Николаевич';
SELECT @Patient6 = Id FROM Patients WHERE FullName = N'Орлова Мария Дмитриевна';
SELECT @Patient7 = Id FROM Patients WHERE FullName = N'Громов Артем Сергеевич';
SELECT @Patient8 = Id FROM Patients WHERE FullName = N'Воронцова Алина Игоревна';
SELECT @Patient9 = Id FROM Patients WHERE FullName = N'Титов Владислав Петрович';
SELECT @Patient10 = Id FROM Patients WHERE FullName = N'Крылова Юлия Александровна';

INSERT INTO Appointments (Id, AppointmentDate, Status, Notes, PatientId, DoctorId)
VALUES
    -- Приемы у других врачей
    (NEWID(), DATEADD(HOUR, 2, GETDATE()), N'Scheduled', N'Плановый осмотр, контроль АД', @Patient1, @Doctor1),
    (NEWID(), DATEADD(DAY, 1, GETDATE()), N'Scheduled', N'Консультация по результатам ЭКГ', @Patient2, @Doctor2),
    (NEWID(), DATEADD(DAY, 2, GETDATE()), N'Scheduled', N'Жалобы на головные боли', @Patient3, @Doctor3),
    (NEWID(), DATEADD(DAY, 3, GETDATE()), N'Scheduled', N'Контроль уровня сахара', @Patient4, @Doctor1),
    
    -- Приемы у Дарины (ее пациенты)
    (NEWID(), DATEADD(DAY, 1, GETDATE()), N'Scheduled', N'Плановый осмотр, жалобы на желудок', @Patient6, @Doctor6),
    (NEWID(), DATEADD(DAY, 2, GETDATE()), N'Scheduled', N'Головокружения, слабость', @Patient7, @Doctor6),
    (NEWID(), DATEADD(DAY, 3, GETDATE()), N'Scheduled', N'Боль в горле, температура', @Patient8, @Doctor6),
    (NEWID(), DATEADD(DAY, 4, GETDATE()), N'Scheduled', N'Низкое давление, обмороки', @Patient9, @Doctor6),
    (NEWID(), DATEADD(DAY, 5, GETDATE()), N'Scheduled', N'Сильные головные боли', @Patient10, @Doctor6),
    
    -- Завершенные приемы
    (NEWID(), DATEADD(DAY, -5, GETDATE()), N'Completed', N'Пациент жалуется на боли в груди', @Patient2, @Doctor2),
    (NEWID(), DATEADD(DAY, -3, GETDATE()), N'Completed', N'Обострение остеохондроза', @Patient3, @Doctor3),
    (NEWID(), DATEADD(DAY, -1, GETDATE()), N'Completed', N'Плановый осмотр, состояние стабильное', @Patient5, @Doctor1),
    
    -- Отмененные приемы
    (NEWID(), DATEADD(DAY, -2, GETDATE()), N'Cancelled', N'Пациент не явился', @Patient1, @Doctor2),
    (NEWID(), DATEADD(DAY, 4, GETDATE()), N'Cancelled', N'Пациент перенес прием', @Patient4, @Doctor3),
    (NEWID(), DATEADD(DAY, 5, GETDATE()), N'Scheduled', N'Повторный прием после лечения', @Patient5, @Doctor2);
GO

-- 6. Добавляем рецепты (12 записей) - получаем ID врачей и пациентов
DECLARE 
    @Doctor1 UNIQUEIDENTIFIER, @Doctor2 UNIQUEIDENTIFIER, @Doctor3 UNIQUEIDENTIFIER,
    @Doctor6 UNIQUEIDENTIFIER,
    @Patient1 UNIQUEIDENTIFIER, @Patient2 UNIQUEIDENTIFIER, @Patient3 UNIQUEIDENTIFIER, 
    @Patient4 UNIQUEIDENTIFIER, @Patient5 UNIQUEIDENTIFIER, @Patient6 UNIQUEIDENTIFIER,
    @Patient7 UNIQUEIDENTIFIER, @Patient8 UNIQUEIDENTIFIER, @Patient9 UNIQUEIDENTIFIER, 
    @Patient10 UNIQUEIDENTIFIER;

-- Получаем ID врачей
SELECT @Doctor1 = Id FROM Doctors WHERE FullName = N'Петров Иван Сергеевич';
SELECT @Doctor2 = Id FROM Doctors WHERE FullName = N'Сидорова Анна Михайловна';
SELECT @Doctor3 = Id FROM Doctors WHERE FullName = N'Козлов Дмитрий Владимирович';
SELECT @Doctor6 = Id FROM Doctors WHERE FullName = N'Дарина Смирнова';

-- Получаем ID пациентов
SELECT @Patient1 = Id FROM Patients WHERE FullName = N'Иванов Сергей Александрович';
SELECT @Patient2 = Id FROM Patients WHERE FullName = N'Смирнова Ольга Викторовна';
SELECT @Patient3 = Id FROM Patients WHERE FullName = N'Кузнецов Михаил Петрович';
SELECT @Patient4 = Id FROM Patients WHERE FullName = N'Попова Екатерина Сергеевна';
SELECT @Patient5 = Id FROM Patients WHERE FullName = N'Федоров Андрей Николаевич';
SELECT @Patient6 = Id FROM Patients WHERE FullName = N'Орлова Мария Дмитриевна';
SELECT @Patient7 = Id FROM Patients WHERE FullName = N'Громов Артем Сергеевич';
SELECT @Patient8 = Id FROM Patients WHERE FullName = N'Воронцова Алина Игоревна';
SELECT @Patient9 = Id FROM Patients WHERE FullName = N'Титов Владислав Петрович';
SELECT @Patient10 = Id FROM Patients WHERE FullName = N'Крылова Юлия Александровна';

INSERT INTO Recipes (Id, IssueDate, Medication, Dosage, Instructions, DurationDays, PatientId, DoctorId)
VALUES
    -- Рецепты от других врачей
    (NEWID(), DATEADD(DAY, -5, GETDATE()), N'Эналаприл', N'5 мг', N'По 1 таблетке утром', 30, @Patient1, @Doctor1),
    (NEWID(), DATEADD(DAY, -5, GETDATE()), N'Аспирин Кардио', N'100 мг', N'По 1 таблетке вечером', 90, @Patient2, @Doctor2),
    (NEWID(), DATEADD(DAY, -3, GETDATE()), N'Диклофенак', N'50 мг', N'По 1 таблетке 2 раза в день после еды', 7, @Patient3, @Doctor3),
    (NEWID(), DATEADD(DAY, -3, GETDATE()), N'Метформин', N'500 мг', N'По 1 таблетке 2 раза в день во время еды', 60, @Patient4, @Doctor1),
    (NEWID(), DATEADD(DAY, -1, GETDATE()), N'Сальбутамол', N'100 мкг/доза', N'По 1-2 ингаляции при приступе', 365, @Patient5, @Doctor1),
    (NEWID(), DATEADD(DAY, -1, GETDATE()), N'Бисопролол', N'2.5 мг', N'По 1 таблетке утром', 30, @Patient2, @Doctor2),
    
    -- Рецепты от Дарины (для ее пациентов)
    (NEWID(), DATEADD(DAY, -2, GETDATE()), N'Омепразол', N'20 мг', N'По 1 капсуле утром за 30 мин до еды', 30, @Patient6, @Doctor6),
    (NEWID(), DATEADD(DAY, -2, GETDATE()), N'Настойка пустырника', N'30 капель', N'По 30 капель 3 раза в день', 14, @Patient7, @Doctor6),
    (NEWID(), DATEADD(DAY, -1, GETDATE()), N'Тонзилгон Н', N'25 капель', N'По 25 капель 3 раза в день', 10, @Patient8, @Doctor6),
    (NEWID(), DATEADD(DAY, -1, GETDATE()), N'Настойка элеутерококка', N'20 капель', N'По 20 капель утром', 21, @Patient9, @Doctor6),
    (NEWID(), GETDATE(), N'Суматриптан', N'50 мг', N'По 1 таблетке при начале мигрени', 10, @Patient10, @Doctor6),
    (NEWID(), GETDATE(), N'Магний B6', N'1 таблетка', N'По 1 таблетке 3 раза в день', 30, @Patient7, @Doctor6);
GO

-- Простая проверка количества записей в таблицах
SELECT N'DoctorTypes' as TableName, COUNT(*) as Count FROM DoctorTypes
UNION ALL
SELECT N'Doctors', COUNT(*) FROM Doctors
UNION ALL
SELECT N'Patients', COUNT(*) FROM Patients
UNION ALL
SELECT N'MedicalCards', COUNT(*) FROM MedicalCards
UNION ALL
SELECT N'Appointments', COUNT(*) FROM Appointments
UNION ALL
SELECT N'Recipes', COUNT(*) FROM Recipes;
GO

-- Детальная проверка каждой таблицы
PRINT N'=== ДЕТАЛЬНАЯ ПРОВЕРКА МЕДИЦИНСКОЙ СИСТЕМЫ ===';

PRINT N'1. Типы врачей:';
SELECT Id, Name, Description FROM DoctorTypes;

PRINT N'2. Врачи (с паролями для входа):';
SELECT d.Id, d.FullName, d.Phone, d.Email, d.Password, dt.Name as Specialization 
FROM Doctors d 
LEFT JOIN DoctorTypes dt ON d.DoctorTypeId = dt.Id;

PRINT N'3. Пациенты:';
SELECT Id, FullName, Phone, Email FROM Patients;

PRINT N'4. Медицинские карты:';
SELECT mc.Id, p.FullName as Patient, mc.MedicalHistory, mc.Allergies, mc.CurrentMedications
FROM MedicalCards mc
LEFT JOIN Patients p ON mc.PatientId = p.Id;

PRINT N'5. Записи на прием:';
SELECT 
    a.Id,
    p.FullName as Patient,
    d.FullName as Doctor,
    a.AppointmentDate,
    a.Status,
    a.Notes
FROM Appointments a
LEFT JOIN Patients p ON a.PatientId = p.Id
LEFT JOIN Doctors d ON a.DoctorId = d.Id
ORDER BY a.AppointmentDate;

PRINT N'6. Рецепты:';
SELECT 
    r.Id,
    p.FullName as Patient,
    d.FullName as Doctor,
    r.Medication,
    r.Dosage,
    r.IssueDate,
    r.DurationDays
FROM Recipes r
LEFT JOIN Patients p ON r.PatientId = p.Id
LEFT JOIN Doctors d ON r.DoctorId = d.Id
ORDER BY r.IssueDate DESC;
GO