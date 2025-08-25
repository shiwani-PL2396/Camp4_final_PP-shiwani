using System.Text.RegularExpressions;

 

namespace ClassLibraryValidation
    {
        public static class PatientValidator
        {
            public static bool IsValidName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return false;

                // Trim leading/trailing spaces
                name = name.Trim();

                // Regex explanation:
                // ^           : Start of string
                // [A-Za-z]+   : First word (only letters)
                // ( [A-Za-z]+)* : Optional following words separated by single spaces
                // $           : End of string
                return Regex.IsMatch(name, @"^[A-Za-z]+( [A-Za-z]+)*$");
            }

            public static bool IsValidBloodGroup(string bloodGroup)
            {
                string[] validGroups = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                return Array.Exists(validGroups, bg => bg.Equals(bloodGroup.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            public static bool IsValidPhoneNumber(string phone)
            {
                return Regex.IsMatch(phone, @"^\d{10}$");
            }

            public static bool IsValidAddress(string address)
            {
                return !string.IsNullOrWhiteSpace(address);
            }




            public static bool IsValidDateOfBirth(DateTime dob)
            {
                DateTime today = DateTime.Today;
                DateTime oldestAllowed = today.AddYears(-130); // 130 years ago

                return dob <= today && dob >= oldestAllowed;
            }

            public static bool IsValidEmergencyContact(string phone)
            {
                return IsValidPhoneNumber(phone); // reuse the same validation
            }

            public static bool AreValidPastDates(DateTime startDate, DateTime endDate)
            {
                DateTime today = DateTime.Today;

                // Check if both dates are in the past or today, and startDate is not after endDate
                return startDate <= today && endDate <= today && startDate <= endDate;
            }

        }
    }
 
