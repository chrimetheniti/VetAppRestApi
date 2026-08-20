-- ============================================================
-- VetDB_TestData_Seed.sql
-- Test data: 5 Veterinarians + 15 Owners + 30 Patients
-- Password for ALL users: Test123!
-- BCrypt hash verified with bcrypt library (workfactor 11)
-- ============================================================

BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================================
    -- STEP 1: Insert Users for Veterinarians (RoleId = 3)
    -- ============================================================
    INSERT INTO [dbo].[Users] ([Username], [Email], [Password], [Firstname], [Lastname], [RoleId], [InsertedAt], [ModifiedAt], [IsDeleted])
    VALUES
        (N'vet_papadopoulos', N'papadopoulos@vetapp.gr', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Nikolaos', N'Papadopoulos', 3, GETUTCDATE(), GETUTCDATE(), 0),
        (N'vet_georgiou', N'georgiou@vetapp.gr', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Eleni', N'Georgiou', 3, GETUTCDATE(), GETUTCDATE(), 0),
        (N'vet_antoniou', N'antoniou@vetapp.gr', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Dimitris', N'Antoniou', 3, GETUTCDATE(), GETUTCDATE(), 0),
        (N'vet_karagianni', N'karagianni@vetapp.gr', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Maria', N'Karagianni', 3, GETUTCDATE(), GETUTCDATE(), 0),
        (N'vet_stefanidis', N'stefanidis@vetapp.gr', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Giorgos', N'Stefanidis', 3, GETUTCDATE(), GETUTCDATE(), 0);

    -- ============================================================
    -- STEP 2: Insert Users for Owners (RoleId = 4)
    -- ============================================================
    INSERT INTO [dbo].[Users] ([Username], [Email], [Password], [Firstname], [Lastname], [RoleId], [InsertedAt], [ModifiedAt], [IsDeleted])
    VALUES
        (N'owner_dimitriou', N'dimitriou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Anna', N'Dimitriou', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_papagiannis', N'papagiannis@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Kostas', N'Papagiannis', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_nikolaou', N'nikolaou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Sofia', N'Nikolaou', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_markakis', N'markakis@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Giannis', N'Markakis', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_rousou', N'rousou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Christina', N'Rousou', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_vasileiou', N'vasileiou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Petros', N'Vasileiou', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_kontou', N'kontou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Eleana', N'Kontou', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_sarris', N'sarris@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Antonis', N'Sarris', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_chatzi', N'chatzi@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Marina', N'Chatzi', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_lampropoulos', N'lampropoulos@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Stelios', N'Lampropoulos', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_aggelou', N'aggelou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Despoina', N'Aggelou', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_miliotis', N'miliotis@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Thanos', N'Miliotis', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_politi', N'politi@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Katerina', N'Politi', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_tzioras', N'tzioras@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Lefteris', N'Tzioras', 4, GETUTCDATE(), GETUTCDATE(), 0),
        (N'owner_kalogirou', N'kalogirou@gmail.com', N'$2b$11$ec5wBKgsNNjLs.eFo26CvujjUCYZSy7o4THr6FuJvdgwchH1HhSAq', N'Zoi', N'Kalogirou', 4, GETUTCDATE(), GETUTCDATE(), 0);

    -- ============================================================
    -- STEP 3: Insert Veterinarians (link to Users via Username)
    -- ============================================================
    INSERT INTO [dbo].[Veterinarians] ([Clinic], [PhoneNumber], [UserId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT v.Clinic, v.Phone, u.[Id], GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Users] u
    INNER JOIN (VALUES
        (N'vet_papadopoulos', N'Ktiniatreio Papadopoulos', N'2109123456'),
        (N'vet_georgiou', N'Zoo-Clinic Athina', N'2109234567'),
        (N'vet_antoniou', N'Ktiniatreio Antoniou Kolonaki', N'2109345678'),
        (N'vet_karagianni', N'Vet Center Glyfadas', N'2109456789'),
        (N'vet_stefanidis', N'Animal Care Peiraia', N'2104567890')
    ) v(Username, Clinic, Phone) ON u.[Username] = v.Username;

    -- ============================================================
    -- STEP 4: Insert Owners (link to Users via Username)
    -- ============================================================
    INSERT INTO [dbo].[Owners] ([Address], [PhoneNumber], [UserId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT o.Address, o.Phone, u.[Id], GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Users] u
    INNER JOIN (VALUES
        (N'owner_dimitriou', N'Papadiamanti 15, Athina', N'6931111111'),
        (N'owner_papagiannis', N'Ermou 42, Kifisia', N'6932222222'),
        (N'owner_nikolaou', N'Vas. Sofias 88, Athina', N'6933333333'),
        (N'owner_markakis', N'Filellinon 5, Glyfada', N'6934444444'),
        (N'owner_rousou', N'Metsovou 12, Voula', N'6935555555'),
        (N'owner_vasileiou', N'Panepistimiou 3, Athina', N'6936666666'),
        (N'owner_kontou', N'Skoufa 27, Kolonaki', N'6937777777'),
        (N'owner_sarris', N'Alkyonis 9, Palaio Faliro', N'6938888888'),
        (N'owner_chatzi', N'Poseidonos 100, Glyfada', N'6939999999'),
        (N'owner_lampropoulos', N'Aristotelous 45, Peristeri', N'6941111111'),
        (N'owner_aggelou', N'Solonos 61, Athina', N'6942222222'),
        (N'owner_miliotis', N'Iera Odos 200, Aigaleo', N'6943333333'),
        (N'owner_politi', N'Kolokotroni 33, Marousi', N'6944444444'),
        (N'owner_tzioras', N'Sygrou 190, N. Smyrni', N'6945555555'),
        (N'owner_kalogirou', N'El. Venizelou 22, Kallithea', N'6946666666')
    ) o(Username, Address, Phone) ON u.[Username] = o.Username;

    -- ============================================================
    -- STEP 5: Insert Patients (link Owner & Vet via their Users)
    -- ============================================================
    INSERT INTO [dbo].[Patients] ([Name], [ChipNumber], [Species], [Breed], [DateOfBirth], [VeterinarianId], [OwnerId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT p.Name, p.Chip, p.Species, p.Breed, p.DOB, vet.[Id], own.[Id], GETUTCDATE(), GETUTCDATE(), 0
    FROM (VALUES
        (N'owner_dimitriou', N'vet_papadopoulos', N'Lucky', N'981000000000001', N'Dog', N'Labrador', '2020-05-14'),
        (N'owner_dimitriou', N'vet_georgiou', N'Miao', N'981000000000002', N'Cat', N'Persian', '2021-03-22'),
        (N'owner_papagiannis', N'vet_papadopoulos', N'Rex', N'981000000000003', N'Dog', N'Rottweiler', '2019-11-08'),
        (N'owner_papagiannis', N'vet_karagianni', N'Bella', N'981000000000004', N'Dog', N'Golden Retriever', '2022-01-30'),
        (N'owner_nikolaou', N'vet_antoniou', N'Whiskers', N'981000000000005', N'Cat', N'Siamese', '2018-07-12'),
        (N'owner_nikolaou', N'vet_stefanidis', N'Coco', N'981000000000006', N'Cat', N'British Shorthair', '2023-02-18'),
        (N'owner_markakis', N'vet_papadopoulos', N'Buddy', N'981000000000007', N'Dog', N'Beagle', '2021-09-05'),
        (N'owner_markakis', N'vet_georgiou', N'Snowball', NULL, N'Rabbit', N'Angora', '2023-04-20'),
        (N'owner_rousou', N'vet_karagianni', N'Max', N'981000000000009', N'Dog', N'German Shepherd', '2020-12-01'),
        (N'owner_rousou', N'vet_antoniou', N'Tweety', NULL, N'Bird', N'Canary', '2024-01-15'),
        (N'owner_vasileiou', N'vet_stefanidis', N'Milo', N'981000000000011', N'Cat', N'Maine Coon', '2019-06-25'),
        (N'owner_vasileiou', N'vet_papadopoulos', N'Charlie', N'981000000000012', N'Dog', N'Boxer', '2022-08-14'),
        (N'owner_kontou', N'vet_georgiou', N'Daisy', N'981000000000013', N'Dog', N'Poodle', '2021-11-30'),
        (N'owner_kontou', N'vet_karagianni', N'Peanut', NULL, N'Hamster', N'Syrian', '2024-03-10'),
        (N'owner_sarris', N'vet_antoniou', N'Rocky', N'981000000000015', N'Dog', N'Bulldog', '2020-04-18'),
        (N'owner_sarris', N'vet_stefanidis', N'Kiwi', NULL, N'Bird', N'Parakeet', '2023-10-05'),
        (N'owner_chatzi', N'vet_papadopoulos', N'Luna', N'981000000000017', N'Cat', N'Ragdoll', '2022-05-22'),
        (N'owner_chatzi', N'vet_georgiou', N'Simba', N'981000000000018', N'Cat', N'Bengal', '2021-07-11'),
        (N'owner_lampropoulos', N'vet_karagianni', N'Zeus', N'981000000000019', N'Dog', N'Siberian Husky', '2019-02-28'),
        (N'owner_lampropoulos', N'vet_antoniou', N'Athena', N'981000000000020', N'Dog', N'Siberian Husky', '2019-02-28'),
        (N'owner_aggelou', N'vet_stefanidis', N'Ginger', N'981000000000021', N'Cat', N'Scottish Fold', '2023-01-08'),
        (N'owner_aggelou', N'vet_papadopoulos', N'Bunny', NULL, N'Rabbit', N'Holland Lop', '2024-05-16'),
        (N'owner_miliotis', N'vet_georgiou', N'Duke', N'981000000000023', N'Dog', N'Doberman', '2020-10-03'),
        (N'owner_miliotis', N'vet_karagianni', N'Tank', NULL, N'Turtle', N'Red-eared Slider', '2015-06-01'),
        (N'owner_politi', N'vet_antoniou', N'Oliver', N'981000000000025', N'Cat', N'Turkish Van', '2022-03-19'),
        (N'owner_politi', N'vet_stefanidis', N'Coco', N'981000000000026', N'Dog', N'Chihuahua', '2023-08-27'),
        (N'owner_tzioras', N'vet_papadopoulos', N'Balto', N'981000000000027', N'Dog', N'Great Dane', '2021-04-12'),
        (N'owner_tzioras', N'vet_georgiou', N'Mochi', N'981000000000028', N'Cat', N'Sphynx', '2022-11-25'),
        (N'owner_kalogirou', N'vet_karagianni', N'Chloe', N'981000000000029', N'Cat', N'Norwegian Forest', '2020-09-14'),
        (N'owner_kalogirou', N'vet_antoniou', N'Nutmeg', NULL, N'Guinea Pig', N'Abyssinian', '2023-12-02')
    ) p(OwnerUsername, VetUsername, Name, Chip, Species, Breed, DOB)
    INNER JOIN [dbo].[Users] ownU ON ownU.[Username] = p.OwnerUsername
    INNER JOIN [dbo].[Owners] own ON own.[UserId] = ownU.[Id]
    INNER JOIN [dbo].[Users] vetU ON vetU.[Username] = p.VetUsername
    INNER JOIN [dbo].[Veterinarians] vet ON vet.[UserId] = vetU.[Id];

    COMMIT TRANSACTION;
    PRINT 'Seed completed: 5 Veterinarians + 15 Owners + 30 Patients';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;