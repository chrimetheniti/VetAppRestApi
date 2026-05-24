BEGIN TRY
    BEGIN TRANSACTION;
    -- ============================================
    -- VetDB - Seed Data
    -- Roles, Capabilities, Role-Capability mappings
    -- ============================================
    
    -- ============================================
    -- Insert Roles
    -- ============================================
    INSERT INTO [dbo].[Roles] ([Name])
    VALUES
        ('ADMIN'),
        ('RECEPTIONIST'),
        ('VETERINARIAN'),
        ('OWNER');
    
    -- ============================================
    -- Insert Capabilities
    -- ============================================
    INSERT INTO [dbo].[Capabilities] ([Name], [Description])
    VALUES
        ('INSERT_VETERINARIAN', 'Create a new veterinarian'),
        ('VIEW_VETERINARIANS', 'View veterinarian list and details'),
        ('VIEW_VETERINARIAN', 'View veterinarian'),
        ('EDIT_VETERINARIAN', 'Modify existing veterinarian'),
        ('DELETE_VETERINARIAN', 'Remove a veterinarian'),
        ('VIEW_ONLY_VETERINARIAN', 'View only own veterinarian details'),
        ('INSERT_PATIENT', 'Create a new patient'),
        ('VIEW_PATIENTS', 'View patient list and details'),
        ('VIEW_PATIENT', 'View patient'),
        ('EDIT_PATIENT', 'Modify existing patient'),
        ('DELETE_PATIENT', 'Remove a patient'),
        ('VIEW_ONLY_PATIENT', 'View only own patient details'),
        ('INSERT_OWNER', 'Create a new owner'),
        ('VIEW_OWNERS', 'View owner list and details'),
        ('VIEW_OWNER', 'View owner'),
        ('EDIT_OWNER', 'Modify existing owner'),
        ('DELETE_OWNER', 'Remove an owner'),
        ('VIEW_ONLY_OWNER', 'View only own owner details');
    
    
    -- ============================================
    -- ADMIN: all capabilities
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'ADMIN';
    
    
    -- ============================================
    -- RECEPTIONIST: view all entities + full CRUD on patients
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'RECEPTIONIST'
      AND c.[Name] IN ('VIEW_VETERINARIANS', 'VIEW_VETERINARIAN',
                       'VIEW_PATIENTS', 'VIEW_PATIENT',
                       'INSERT_PATIENT', 'EDIT_PATIENT', 'DELETE_PATIENT',
                       'VIEW_OWNERS', 'VIEW_OWNER');
    
    
    -- ============================================
    -- VETERINARIAN: VIEW_ONLY_VETERINARIAN
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'VETERINARIAN'
      AND c.[Name] IN ('VIEW_ONLY_VETERINARIAN');
    
    
    -- ============================================
    -- OWNER: VIEW_ONLY_OWNER
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'OWNER'
      AND c.[Name] IN ('VIEW_ONLY_OWNER');
        
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;

DBCC CHECKIDENT ('dbo.Roles', RESEED, 4);
DBCC CHECKIDENT ('dbo.Capabilities', RESEED, 18); -- το επόμενο INSERT θα παράγει 19.
