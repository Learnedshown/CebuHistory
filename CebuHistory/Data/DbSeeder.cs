using Microsoft.AspNetCore.Identity;
using CebuHistory.Models;

namespace CebuHistory.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // ==========================================
        // SEED ROLES
        // ==========================================
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ==========================================
        // SEED DEFAULT ADMIN USER
        // ==========================================
        const string adminEmail = "admin@sugbo.ph";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Sugbo",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true,
            };
            var result = await userManager.CreateAsync(admin, "Admin@1234");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ==========================================
        // SEED TIMELINE EVENTS
        // ==========================================
        if (!context.TimelineEvents.Any())
        {
            var timelineEvents = new[]
            {
                new TimelineEvent { Year = 1521, Title = "Magellan Arrives in Cebu", Era = "Spanish Contact", Description = "Ferdinand Magellan lands in Cebu on April 7, 1521. Rajah Humabon is baptized.", Importance = 10 },
                new TimelineEvent { Year = 1565, Title = "Legazpi Founds the Colony", Era = "Spanish Colonial", Description = "Miguel López de Legazpi establishes the first permanent Spanish settlement.", Importance = 9 },
                new TimelineEvent { Year = 1898, Title = "Leon Kilat's Uprising", Era = "Revolution", Description = "Pantaleon Villegas leads the April 3 uprising against Spanish rule.", Importance = 8 },
                new TimelineEvent { Year = 1942, Title = "Japanese Occupation Begins", Era = "World War II", Description = "Japanese forces enter Cebu City on April 10, 1942.", Importance = 9 },
                new TimelineEvent { Year = 1945, Title = "Liberation of Cebu", Era = "World War II", Description = "American and Filipino forces land on Cebu on March 26, 1945.", Importance = 9 },
                new TimelineEvent { Year = 1980, Title = "Sinulog Festival Formalized", Era = "Modern", Description = "The Sinulog Festival is officially organized as a grand cultural celebration.", Importance = 7 },
            };
            await context.TimelineEvents.AddRangeAsync(timelineEvents);
        }

        // ==========================================
        // SEED STORIES
        // ==========================================
        if (!context.Stories.Any())
        {
            var stories = new[]
            {
                new Story
                {
                    Title = "Magellan's Cross and the Baptism of Rajah Humabon",
                    Slug = "magellans-cross-baptism-humabon",
                    Era = "Spanish Colonial",
                    Category = "Religion & Conversion",
                    Author = "Dr. Maria Santos",
                    PublishedAt = new DateTime(2024, 3, 15),
                    CoverImage = "/uploads/stories/cross.jpg",
                    Excerpt = "On April 14, 1521, a wooden cross was planted on the shores of what would become Cebu City — an act that changed the spiritual landscape of an archipelago forever.",
                    Body = @"On April 14, 1521, Ferdinand Magellan planted a cross on the shores of Zubu — today's Cebu City — to commemorate the mass baptism of Rajah Humabon, his wife Hara Humamay (christened Juana), and some 800 of their subjects. It was the first Christian baptism in the Philippines and one of the most consequential moments in Southeast Asian history.

The cross, now enshrined inside a chapel near the Basilica Minore del Santo Niño, has been encased in tindalo wood to protect it from relic-seekers who once chipped pieces away believing it had miraculous powers. Today it remains one of Cebu's most visited landmarks.",
                    Tags = "Magellan,Humabon,Spanish,1521",
                    IsFeatured = true,
                    Status = "Published",
                    ReadingTime = 5,
                    Views = 0,
                    CreatedAt = DateTime.UtcNow,
                },
                new Story
                {
                    Title = "The Battle of Mactan: Lapu-Lapu and the First Resistance",
                    Slug = "battle-of-mactan-lapu-lapu",
                    Era = "Pre-Colonial / Spanish Contact",
                    Category = "War & Resistance",
                    Author = "Prof. Jose Reyes",
                    PublishedAt = new DateTime(2024, 2, 28),
                    CoverImage = "/uploads/stories/mactan.avif",
                    Excerpt = "On the morning of April 27, 1521, a chieftain named Lapu-Lapu led his warriors against Ferdinand Magellan's armored soldiers — and won.",
                    Body = @"The shallow waters off Mactan Island turned the tide of history on April 27, 1521. Ferdinand Magellan, emboldened by his alliance with Rajah Humabon, made the fatal decision to punish Lapu-Lapu of Mactan for refusing to submit to Spanish authority.

Magellan brought roughly 49 men ashore in the early morning. The coral reef prevented his ships from getting close enough to provide covering fire. Today Lapu-Lapu is celebrated as the first Filipino hero, the first to resist colonial conquest.",
                    Tags = "Lapu-Lapu,Mactan,Magellan,1521",
                    IsFeatured = true,
                    Status = "Published",
                    ReadingTime = 5,
                    Views = 0,
                    CreatedAt = DateTime.UtcNow,
                },
                new Story
                {
                    Title = "The Santo Niño: Cebu's Most Sacred Relic",
                    Slug = "santo-nino-cebu-relic",
                    Era = "Spanish Colonial",
                    Category = "Religion & Culture",
                    Author = "Fr. Antonio dela Cruz",
                    PublishedAt = new DateTime(2024, 1, 10),
                    CoverImage = "/uploads/stories/santonino.jpg",
                    Excerpt = "When Miguel López de Legazpi arrived in 1565, a soldier found something extraordinary inside a burning house — a small wooden image of the Child Jesus, unscathed.",
                    Body = @"The image of the Santo Niño de Cebú is the oldest Roman Catholic religious relic in the Philippines. Gifted by Magellan to Hara Humamay upon her baptism in 1521, it was presumed lost after relations between the Spanish and Cebuanos broke down following Magellan's death. Forty-four years later, it was found.

The image itself — just 27 centimeters tall, carved from dark wood in the Flemish style — shows the Child Jesus holding an orb and wearing a small crown. Every January, millions descend on Cebu for the Sinulog Festival — nine days of processions, street dancing, and mass celebrating the Santo Niño.",
                    Tags = "Santo Niño,Sinulog,Legazpi,Basilica",
                    IsFeatured = true,
                    Status = "Published",
                    ReadingTime = 5,
                    Views = 0,
                    CreatedAt = DateTime.UtcNow,
                },
                new Story
                {
                    Title = "Fort San Pedro: The Oldest Fort in the Philippines",
                    Slug = "fort-san-pedro-cebu",
                    Era = "Spanish Colonial",
                    Category = "Architecture & Heritage",
                    Author = "Dr. Maria Santos",
                    PublishedAt = new DateTime(2024, 4, 15),
                    CoverImage = "/uploads/stories/fort.jpg",
                    Excerpt = "Built in 1738, this triangular fortress has witnessed over 280 years of Cebu's turbulent history — from Spanish galleons to American soldiers to Japanese occupation.",
                    Body = @"Fort San Pedro is a military defense structure built by Spanish and indigenous Cebuano laborers under the command of Miguel López de Legazpi. Located in the area now called Plaza Independencia, it is the oldest and smallest fort in the Philippines.

Today, Fort San Pedro is open to the public as a historical park and museum. Visitors can explore the original cannons still pointing out to sea, a small museum displaying Spanish artifacts, and panoramic views of Cebu's pier and harbor.",
                    Tags = "Fort,San Pedro,Spanish,Architecture",
                    IsFeatured = false,
                    Status = "Published",
                    ReadingTime = 5,
                    Views = 0,
                    CreatedAt = DateTime.UtcNow,
                },
            };
            await context.Stories.AddRangeAsync(stories);
        }

        // ==========================================
        // SEED PHOTOS
        // ==========================================
        if (!context.Photos.Any())
        {
            var photos = new[]
            {
                new Photo
                {
                    Title = "Cebu City circa 1900",
                    Era = "American Period",
                    Year = 1900,
                    Location = "Cebu City",
                    Source = "U.S. National Archives",
                    Description = "A view of the Parian district showing the old Chinese mestizo quarter and the campanario of the Santo Niño Basilica rising behind low rooftops.",
                    ImageUrl = "/uploads/photos/circa.jpg",
                    IsPublicDomain = true,
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new Photo
                {
                    Title = "Basilica Minore del Santo Niño, 1910s",
                    Era = "American Period",
                    Year = 1915,
                    Location = "Cebu City",
                    Source = "Philippine National Library",
                    Description = "The basilica as it appeared in the early American period, before the 1939 renovation that added its current facade.",
                    ImageUrl = "/uploads/photos/basilica.jpg",
                    IsPublicDomain = true,
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new Photo
                {
                    Title = "Magellan's Cross Pavilion, 1920s",
                    Era = "American Period",
                    Year = 1922,
                    Location = "Cebu City",
                    Source = "Philippine National Library",
                    Description = "The octagonal pavilion sheltering Magellan's Cross, built in the early 20th century to replace earlier wooden structures.",
                    ImageUrl = "/uploads/photos/mactan.avif",
                    IsPublicDomain = true,
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new Photo
                {
                    Title = "Fort San Pedro, 1898",
                    Era = "Spanish Colonial",
                    Year = 1898,
                    Location = "Cebu City",
                    Source = "Archivo General de Indias",
                    Description = "The oldest and smallest Spanish fort in the Philippines, originally built in 1565 by Miguel López de Legazpi.",
                    ImageUrl = "/uploads/photos/fort.jpg",
                    IsPublicDomain = true,
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new Photo
                {
                    Title = "Sinulog Procession, 1955",
                    Era = "Modern",
                    Year = 1955,
                    Location = "Cebu City",
                    Source = "Cebu Heritage Foundation",
                    Description = "Devotees carry the image of the Santo Niño through the streets of Cebu in the annual January procession, decades before Sinulog became the grand festival it is today.",
                    ImageUrl = "/uploads/photos/sinulog.jpg",
                    IsPublicDomain = true,
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
            };
            await context.Photos.AddRangeAsync(photos);
        }

        // ==========================================
        // SEED DOCUMENTS
        // ==========================================
        if (!context.Documents.Any())
        {
            var documents = new[]
            {
                new HistoricalDocument
                {
                    Title = "Pigafetta's Account of the Cebu Baptism, 1521",
                    Era = "Spanish Colonial",
                    Year = 1521,
                    DocumentType = "Chronicle",
                    Source = "Biblioteca Ambrosiana, Milan",
                    Description = "Antonio Pigafetta's firsthand account of the baptism of Rajah Humabon and the planting of the cross, from his chronicle of Magellan's circumnavigation.",
                    DocumentUrl = "/uploads/documents/pigafetta.pdf",
                    ThumbnailUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/35/Pigafetta_manuscript.jpg/400px-Pigafetta_manuscript.jpg",
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new HistoricalDocument
                {
                    Title = "Spanish Map of Cebu Island, 1734",
                    Era = "Spanish Colonial",
                    Year = 1734,
                    DocumentType = "Map",
                    Source = "Archivo General de Indias, Seville",
                    Description = "A detailed Spanish colonial map showing the settlements, missions, and coastal features of Cebu island, produced for the colonial administration.",
                    DocumentUrl = "/uploads/documents/map.jpg",
                    ThumbnailUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/Cebu_1734_map.jpg/400px-Cebu_1734_map.jpg",
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new HistoricalDocument
                {
                    Title = "Decree Founding Cebu as a City, 1937",
                    Era = "American Period",
                    Year = 1937,
                    DocumentType = "Decree",
                    Source = "National Archives of the Philippines",
                    Description = "Commonwealth Act No. 58 signed by President Manuel Quezon, formally constituting Cebu as a chartered city, the first outside Luzon.",
                    DocumentUrl = "/uploads/documents/foundingofcebu.pdf",
                    ThumbnailUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4c/Commonwealth_Act.jpg/400px-Commonwealth_Act.jpg",
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
                new HistoricalDocument
                {
                    Title = "Japanese Occupation Proclamation, 1942",
                    Era = "World War II",
                    Year = 1942,
                    DocumentType = "Proclamation",
                    Source = "Imperial Japanese Army Records",
                    Description = "The proclamation issued by Japanese military authorities upon the occupation of Cebu, announcing the terms of the military administration.",
                    DocumentUrl = "/uploads/documents/japan.pdf",
                    ThumbnailUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Japanese_proclamation.jpg/400px-Japanese_proclamation.jpg",
                    Status = "Published",
                    UploadedAt = DateTime.UtcNow,
                },
            };
            await context.Documents.AddRangeAsync(documents);
        }

        await context.SaveChangesAsync();
    }
}