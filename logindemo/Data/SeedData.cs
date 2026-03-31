using logindemo.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace logindemo.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedUsersAndRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // ✅ Step 1: Ensure roles exist
            string[] roles = { "Admin", "User", "Supervisor" };
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ✅ Step 2: Define Admin Accounts with Specific Passwords
            var admins = new[]
            {
                new { UserName = "vigneshcctns", Email = "vignesh@cctns.com", FullName = "CCTNS VIGNESH KUMAR", Password = "@Cctns1234" },
                new { UserName = "pushparajcctns", Email = "pushparaj@cctns.com", FullName = "CCTNS PUSHPA RAJ", Password = "@Cctns1234" },
                new { UserName = "ramkumarcctns", Email = "ramkumar@cctns.com", FullName = "CCTNS RAM KUMAR", Password = "@Cctns1234" },
                new { UserName = "chandrucctns", Email = "chandru@cctns.com", FullName = "CCTNS CHANDRU", Password = "@Cctns1234" },
                new { UserName = "teamcctns", Email = "cctnsteam@cctns.com", FullName = "CCTNS TEAM", Password = "@Cctns1234" },
                // 👇 UNIQUE PASSWORDS SET HERE
                new { UserName = "cctnscell1", Email = "cctnscell1@cctns.com", FullName = "CCTNS CELL 01", Password = "cctnscell@01" },
                new { UserName = "cctnscell2", Email = "cctnscell2@cctns.com", FullName = "CCTNS CELL 02", Password = "cctnscell@02" },
                new { UserName = "nodalofficer", Email = "nodalofficer@cctns.com", FullName = "NODAL OFFICER", Password = "nodalofficer@01" }
            };

            foreach (var admin in admins)
            {
                var existing = await userManager.FindByNameAsync(admin.UserName);
                if (existing != null)
                {
                    // Update existing admin or delete/recreate. Using Delete/Recreate to ensure clean state as per your original logic.
                    await userManager.DeleteAsync(existing);
                    Console.WriteLine($"🗑️ Resetting admin user: {admin.UserName}");
                }

                var newUser = new ApplicationUser
                {
                    UserName = admin.UserName,
                    Email = admin.Email,
                    EmailConfirmed = true,
                    FullName = admin.FullName,
                    PoliceStationCode = "ADMIN",
                    NormalizedEmail = admin.Email.ToUpperInvariant(),
                    NormalizedUserName = admin.UserName.ToUpperInvariant()
                };

                var result = await userManager.CreateAsync(newUser, admin.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                    Console.WriteLine($"✅ Created admin: {admin.UserName} with specific password.");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to create admin {admin.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var policeStations = new[]
               {
                    new { StationCode = "26001001", StationName = "FOODCELLPSKARAIKAL" },
                    new { StationCode = "26001002", StationName = "KOTTUCHERRYPS" },
                    new { StationCode = "26001003", StationName = "TOWNKARAIKALPS" },
                    new { StationCode = "26001004", StationName = "NEDUNGADUPS" },
                    new { StationCode = "26001005", StationName = "NERAVYPS" },
                    new { StationCode = "26001006", StationName = "THIRUNALLARPS" },
                    new { StationCode = "26001007", StationName = "TRPATTINAMPS" },
                    new { StationCode = "26001008", StationName = "WOMENPSKARAIKAL" },
                    new { StationCode = "26001009", StationName = "PCRCELLPSKARAIKAL" },
                    new { StationCode = "26001010", StationName = "TRAFFICNORTHKARAIKALPS" },
                    new { StationCode = "26001011", StationName = "EXCISEPSKARAIKAL" },
                    new { StationCode = "26001012", StationName = "COASTALPSKARAIKAL" },
                    new { StationCode = "26001013", StationName = "TRAFFICSOUTHKARAIKALPS" },
                    new { StationCode = "26002001", StationName = "VACPS" },
                    new { StationCode = "26002002", StationName = "CBCIDPS" },
                    new { StationCode = "26002003", StationName = "DNAGARPS" },
                    new { StationCode = "26002004", StationName = "REDDIARPALAYAMPS" },
                    new { StationCode = "26002005", StationName = "MUDALIARPETPS" },
                    new { StationCode = "26002006", StationName = "METTUPALAYAMPS" },
                    new { StationCode = "26002007", StationName = "VILLIANURPS" },
                    new { StationCode = "26002008", StationName = "WOMENPSVILLIANUR" },
                    new { StationCode = "26002009", StationName = "MANGALAMPS" },
                    new { StationCode = "26002010", StationName = "FOODCELLPSPUDUCHERRY" },
                    new { StationCode = "26002011", StationName = "MAHEPS" },
                    new { StationCode = "26002012", StationName = "PALLOORPS" },
                    new { StationCode = "26002013", StationName = "YANAMPS" },
                    new { StationCode = "26002014", StationName = "PCRCELLPSPUDUCHERRY" },
                    new { StationCode = "26002015", StationName = "PCRCELLPSYANAM" },
                    new { StationCode = "26002016", StationName = "GRANDBAZARPS" },
                    new { StationCode = "26002017", StationName = "KALAPETPS" },
                    new { StationCode = "26002018", StationName = "MUTHIALPETPS" },
                    new { StationCode = "26002019", StationName = "LAWSPETPS" },
                    new { StationCode = "26002020", StationName = "ODIANSALAIPS" },
                    new { StationCode = "26002021", StationName = "ORLEANPETPS" },
                    new { StationCode = "26002022", StationName = "AWPSPUDUCHERRY" },
                    new { StationCode = "26002023", StationName = "TRAFFICEASTPUDUCHERRYPS" },
                    new { StationCode = "26002024", StationName = "TRAFFICWESTPUDUCHERRYPS" },
                    new { StationCode = "26002025", StationName = "ARIANKUPPAMPS" },
                    new { StationCode = "26002026", StationName = "SEDARAPETPS" },
                    new { StationCode = "26002027", StationName = "BAHOURPS" },
                    new { StationCode = "26002028", StationName = "KIRUMAMPAKKAMPS" },
                    new { StationCode = "26002029", StationName = "KATTERIKUPPAMPS" },
                    new { StationCode = "26002030", StationName = "NETTAPAKKAMPS" },
                    new { StationCode = "26002031", StationName = "THAVALAKUPPAMPS" },
                    new { StationCode = "26002032", StationName = "THIRUBUVANAIPS" },
                    new { StationCode = "26002033", StationName = "THIRUKANURPS" },
                    new { StationCode = "26002034", StationName = "EXCISEPSMAHE" },
                    new { StationCode = "26002035", StationName = "EXCISEPSYANAM" },
                    new { StationCode = "26002036", StationName = "TRAFFICSOUTHPUDUCHERRYPS" },
                    new { StationCode = "26002037", StationName = "EXCISEPSPUDUCHERRY" },
                    new { StationCode = "26002038", StationName = "COASTALPSPUDUCHERRY" },
                    new { StationCode = "26002039", StationName = "COASTALPSMAHE" },
                    new { StationCode = "26002040", StationName = "COASTALPSYANAM" },
                    new { StationCode = "26002041", StationName = "TRAFFICNORTHPUDUCHERRYPS" },
                    new { StationCode = "26002042", StationName = "PSECONOMICOFFENSESWING" },
                    new { StationCode = "26002043", StationName = "CYBERCRIMECELLPS" },
                    new { StationCode = "26002044", StationName = "TRAFFICYANAMPS" }
                };

            foreach (var station in policeStations)
            {
                string username = station.StationName.Trim();
                string normalizedUsername = username.ToUpperInvariant();
                string displayName = station.StationName.Replace("PS", " Police Station").Replace("CELL", " Cell").Trim();

                // Dynamic password generation
                string lastDigits = station.StationCode.Length >= 4
                    ? station.StationCode.Substring(station.StationCode.Length - 4)
                    : station.StationCode;
                string password = $"cctns@{lastDigits}";

                var existingUser = userManager.Users.FirstOrDefault(u => u.PoliceStationCode == station.StationCode);

                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = username,
                        NormalizedUserName = normalizedUsername,
                        EmailConfirmed = true,
                        FullName = displayName,
                        PoliceStationCode = station.StationCode
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        Console.WriteLine($"✅ Created station user: {username}");
                    }
                }
                else
                {
                    // Update user details if station exists
                    existingUser.UserName = username;
                    existingUser.FullName = displayName;
                    await userManager.UpdateAsync(existingUser);

                    // Reset password to sync with current StationCode logic
                    var token = await userManager.GeneratePasswordResetTokenAsync(existingUser);
                    await userManager.ResetPasswordAsync(existingUser, token, password);

                    if (!await userManager.IsInRoleAsync(existingUser, "User"))
                        await userManager.AddToRoleAsync(existingUser, "User");

                    Console.WriteLine($"🔄 Updated station user: {username}");
                }
            }
        }
    }
}















/*using logindemo.Models;
using Microsoft.AspNetCore.Identity;


namespace logindemo.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedUsersAndRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // ✅ Step 1: Ensure roles exist
            string[] roles = { "Admin", "User", "Supervisor" };
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }


            // ✅ Step 2: Default admin password
            string defaultPassword = "@Cctns1234";

            // ✅ Step 3: Define admin accounts
            var admins = new[]
            {
                new { UserName = "vigneshcctns", Email = "vignesh@cctns.com", FullName = "CCTNS VIGNESH KUMAR" },
                new { UserName = "pushparajcctns", Email = "pushparaj@cctns.com", FullName = "CCTNS PUSHPA RAJ" },
                 new { UserName = "ramkumarcctns", Email = "ramkumar@cctns.com", FullName = "CCTNS RAM KUMAR" },
                new { UserName = "chandrucctns", Email = "chandru@cctns.com", FullName = "CCTNS CHANDRU" },
                new { UserName = "teamcctns", Email = "cctnsteam@cctns.com", FullName = "CCTNS TEAM" }
            };

            foreach (var admin in admins)
            {
                var existing = await userManager.FindByEmailAsync(admin.Email);
                if (existing != null)
                {
                    await userManager.DeleteAsync(existing);
                    Console.WriteLine($"🗑️ Deleted existing admin user: {admin.Email}");
                }

                var newUser = new ApplicationUser
                {
                    UserName = admin.UserName,
                    Email = admin.Email,
                    EmailConfirmed = true,
                    FullName = admin.FullName,
                    PoliceStationCode = "ADMIN",
                    NormalizedEmail = admin.Email.ToUpperInvariant(),
                    NormalizedUserName = admin.UserName.ToUpperInvariant()
                };

                var result = await userManager.CreateAsync(newUser, defaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                    Console.WriteLine($"✅ Created admin: {admin.UserName}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to create admin {admin.UserName}: {string.Join(", ", result.Errors)}");
                }
            }

            //// ✅ Step 4: Create 10 default user accounts if not exist
            //for (int i = 1; i <= 10; i++)
            //{
            //    string username = $"user{i}";

            //    var existingUser = await userManager.FindByNameAsync(username);
            //    if (existingUser == null)
            //    {
            //        var user = new ApplicationUser
            //        {
            //            UserName = username,
            //            // Email = null or empty since not used
            //            EmailConfirmed = true,
            //            FullName = $"Demo User {i}",
            //            PoliceStationCode = $"PS00{i}"
            //        };

            //        var result = await userManager.CreateAsync(user, "User@1234");
            //        if (result.Succeeded)
            //        {
            //            await userManager.AddToRoleAsync(user, "User");
            //            Console.WriteLine($"👤 Created default user: {username}");
            //        }
            //        else
            //        {
            //            Console.WriteLine($"❌ Failed to create default user {username}: {string.Join(", ", result.Errors)}");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"ℹ️ User already exists: {username}");
            //    }
            //}

            var policeStations = new[]
                {
                    new { StationCode = "26001001", StationName = "FOODCELLPSKARAIKAL" },
                    new { StationCode = "26001002", StationName = "KOTTUCHERRYPS" },
                    new { StationCode = "26001003", StationName = "TOWNKARAIKALPS" },
                    new { StationCode = "26001004", StationName = "NEDUNGADUPS" },
                    new { StationCode = "26001005", StationName = "NERAVYPS" },
                    new { StationCode = "26001006", StationName = "THIRUNALLARPS" },
                    new { StationCode = "26001007", StationName = "TRPATTINAMPS" },
                    new { StationCode = "26001008", StationName = "WOMENPSKARAIKAL" },
                    new { StationCode = "26001009", StationName = "PCRCELLPSKARAIKAL" },
                    new { StationCode = "26001010", StationName = "TRAFFICNORTHKARAIKALPS" },
                    new { StationCode = "26001011", StationName = "EXCISEPSKARAIKAL" },
                    new { StationCode = "26001012", StationName = "COASTALPSKARAIKAL" },
                    new { StationCode = "26001013", StationName = "TRAFFICSOUTHKARAIKALPS" },
                    new { StationCode = "26002001", StationName = "VACPS" },
                    new { StationCode = "26002002", StationName = "CBCIDPS" },
                    new { StationCode = "26002003", StationName = "DNAGARPS" },
                    new { StationCode = "26002004", StationName = "REDDIARPALAYAMPS" },
                    new { StationCode = "26002005", StationName = "MUDALIARPETPS" },
                    new { StationCode = "26002006", StationName = "METTUPALAYAMPS" },
                    new { StationCode = "26002007", StationName = "VILLIANURPS" },
                    new { StationCode = "26002008", StationName = "WOMENPSVILLIANUR" },
                    new { StationCode = "26002009", StationName = "MANGALAMPS" },
                    new { StationCode = "26002010", StationName = "FOODCELLPSPUDUCHERRY" },
                    new { StationCode = "26002011", StationName = "MAHEPS" },
                    new { StationCode = "26002012", StationName = "PALLOORPS" },
                    new { StationCode = "26002013", StationName = "YANAMPS" },
                    new { StationCode = "26002014", StationName = "PCRCELLPSPUDUCHERRY" },
                    new { StationCode = "26002015", StationName = "PCRCELLPSYANAM" },
                    new { StationCode = "26002016", StationName = "GRANDBAZARPS" },
                    new { StationCode = "26002017", StationName = "KALAPETPS" },
                    new { StationCode = "26002018", StationName = "MUTHIALPETPS" },
                    new { StationCode = "26002019", StationName = "LAWSPETPS" },
                    new { StationCode = "26002020", StationName = "ODIANSALAIPS" },
                    new { StationCode = "26002021", StationName = "ORLEANPETPS" },
                    new { StationCode = "26002022", StationName = "AWPSPUDUCHERRY" },
                    new { StationCode = "26002023", StationName = "TRAFFICEASTPUDUCHERRYPS" },
                    new { StationCode = "26002024", StationName = "TRAFFICWESTPUDUCHERRYPS" },
                    new { StationCode = "26002025", StationName = "ARIANKUPPAMPS" },
                    new { StationCode = "26002026", StationName = "SEDARAPETPS" },
                    new { StationCode = "26002027", StationName = "BAHOURPS" },
                    new { StationCode = "26002028", StationName = "KIRUMAMPAKKAMPS" },
                    new { StationCode = "26002029", StationName = "KATTERIKUPPAMPS" },
                    new { StationCode = "26002030", StationName = "NETTAPAKKAMPS" },
                    new { StationCode = "26002031", StationName = "THAVALAKUPPAMPS" },
                    new { StationCode = "26002032", StationName = "THIRUBUVANAIPS" },
                    new { StationCode = "26002033", StationName = "THIRUKANURPS" },
                    new { StationCode = "26002034", StationName = "EXCISEPSMAHE" },
                    new { StationCode = "26002035", StationName = "EXCISEPSYANAM" },
                    new { StationCode = "26002036", StationName = "TRAFFICSOUTHPUDUCHERRYPS" },
                    new { StationCode = "26002037", StationName = "EXCISEPSPUDUCHERRY" },
                    new { StationCode = "26002038", StationName = "COASTALPSPUDUCHERRY" },
                    new { StationCode = "26002039", StationName = "COASTALPSMAHE" },
                    new { StationCode = "26002040", StationName = "COASTALPSYANAM" },
                    new { StationCode = "26002041", StationName = "TRAFFICNORTHPUDUCHERRYPS" },
                    new { StationCode = "26002042", StationName = "PSECONOMICOFFENSESWING" },
                    new { StationCode = "26002043", StationName = "CYBERCRIMECELLPS" },
                    new { StationCode = "26002044", StationName = "TRAFFICYANAMPS" }
                };

            foreach (var station in policeStations)
            {
                string username = station.StationName.Trim();
                string normalizedUsername = username.ToUpperInvariant();

                string displayName = station.StationName
                    .Replace("PS", "Police Station")
                    .Replace("CELL", "Cell")
                    .Trim();

                string lastDigits = station.StationCode.Length >= 4
                    ? station.StationCode.Substring(station.StationCode.Length - 4)
                    : station.StationCode;

                string password = $"cctns@{lastDigits}";

                // 🔍 Find user by PoliceStationCode FIRST (IMPORTANT)
                var existingUser = userManager.Users
                    .FirstOrDefault(u => u.PoliceStationCode == station.StationCode);

                if (existingUser == null)
                {
                    // ✅ CREATE NEW USER
                    var user = new ApplicationUser
                    {
                        UserName = username,
                        NormalizedUserName = normalizedUsername,
                        EmailConfirmed = true,
                        FullName = displayName,
                        PoliceStationCode = station.StationCode
                    };

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        Console.WriteLine($"✅ Created user: {username} | Password: {password}");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"❌ Failed to create user {username}: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                        );
                    }
                }
                else
                {
                    // ✅ UPDATE EXISTING USER (THIS FIXES YOUR ISSUE)
                    existingUser.UserName = username;
                    existingUser.NormalizedUserName = normalizedUsername;
                    existingUser.FullName = displayName;

                    await userManager.UpdateAsync(existingUser);

                    // 🔐 RESET PASSWORD
                    var token = await userManager.GeneratePasswordResetTokenAsync(existingUser);
                    await userManager.ResetPasswordAsync(existingUser, token, password);

                    // ✅ ENSURE ROLE
                    if (!await userManager.IsInRoleAsync(existingUser, "User"))
                    {
                        await userManager.AddToRoleAsync(existingUser, "User");
                    }

                    Console.WriteLine($"🔄 Updated user: {username} | Password reset to {password}");
                }
            }



            // ✅ CLOSE METHOD
        }

        // ✅ CLOSE CLASS
    }

    // ✅ CLOSE NAMESPACE
}*/

