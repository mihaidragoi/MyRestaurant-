using RestaurantIncercareaDoua.Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIncercareaDoua.Business_Logic_Layer
{
    public class UserBLL
    {
        private UserDataAccess userDAL = new UserDataAccess();
        public string CreateAccount(string firstName, string lastName, string email, string phone, string address, string password)
        {
            if (!email.Contains("@"))
            {
                return "Invalid email format.";
            }
            string role = "client";
            try
            {
                userDAL.RegisterUser(firstName, lastName, email, phone, address, password, role);
                return "Account created successfully.";
            }
            catch (Exception ex)
            {
                return "Error creating account: " + ex.Message;
            }
        }

        public string LogIn(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return "Email and password cannot be empty.";
            }
            else
                return userDAL.VerifyUser(email, password) ?? "Invalid email or password.";
        }

        public int GetUserId(string email)
        {
            return userDAL.GetUserIdByEmail(email);
        }
    }
}
