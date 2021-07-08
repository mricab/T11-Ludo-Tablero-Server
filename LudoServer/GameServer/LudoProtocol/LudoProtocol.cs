using System;

namespace LudoProtocol
{
    public abstract class LProtocol
    {
        public static LPackage GetPackage(string actionName, string[] contents = null)
        {
            byte code = ActionCode(actionName);
            switch (actionName)
            {
                case "unknown request":     return new LPackage(code, contents);
                case "success":             return new LPackage(code, contents);
                case "login":               return new LPackage(code, contents);
                case "login failure":       return new LPackage(code, DefaultContent(code));
                case "duplicate login":     return new LPackage(code, DefaultContent(code));
                case "login successful":    return new LPackage(code, contents);
                case "logout":              return new LPackage(code, DefaultContent(code));
                case "logout failure":      return new LPackage(code, DefaultContent(code));
                case "logout successful":   return new LPackage(code, DefaultContent(code));
                case "register":            return new LPackage(code, contents);
                case "register successful": return new LPackage(code, contents);
                case "register failure":    return new LPackage(code, contents);
                case "get board":           return new LPackage(code, contents);
                case "board":               return new LPackage(code, contents);
                default:                    return null;
            }
        }

        public static string ActionName(byte actionCode)
        {
            switch (actionCode)
            {
                case 0:     return "unknown request";
                case 1:     return "success";
                case 10:    return "login";
                case 11:    return "login failure";
                case 12:    return "duplicate login";
                case 13:    return "login successful";
                case 20:    return "logout";
                case 21:    return "logout failure";
                case 22:    return "logout successful";
                case 30:    return "register";
                case 31:    return "register successful";
                case 32:    return "register failure";
                case 40:    return "get board";
                case 41:    return "board";
                default:    return null;
            }
        }

        public static byte ActionCode(string actionName)
        {
            switch (actionName)
            {
                case "unknown request":         return 0;
                case "success":                 return 1;
                case "login":                   return 10;
                case "login failure":           return 11;
                case "duplicate login":         return 12;
                case "login successful":        return 13;
                case "logout":                  return 20;
                case "logout failure":          return 21;
                case "logout successful":       return 22;
                case "register":                return 30;
                case "register successful":     return 31;
                case "register failure":        return 32;
                case "get board":               return 40;
                case "board":                   return 41;
                default:                        return 255; // null
            }
        }

        public static string[] DefaultContent(int actionCode)
        {
            switch (actionCode)
            {
                case 0:     return new string[] { };
                case 11:    return new string[] { "Invalid username or password." };
                case 12:    return new string[] { "Already logged in." };
                case 20:    return new string[] { };
                case 21:    return new string[] { };
                default:    return null;
            }
        }
    }
}
