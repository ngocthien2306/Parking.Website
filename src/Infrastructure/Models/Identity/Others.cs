using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace InfrastructureCore.Models.Identity
{
    public enum CredentialTypes
    {
        LocalEmail, // Default Login schema
        FaceBook
    }

    public enum ValidateResultError
    {
        NoError,
        SiteNotFound,
        CredentialTypeNotFound,
        CredentialNotFound,
        PasswordNotValid,
        PasswordMostExpired,
        PasswordExpired,
        IsBlocked
    }

    public class ValidateResult
    {
        public SYUser SYUser { get; set; }
        public bool Success { get; set; }
        public ValidateResultError? Error { get; set; }

        public ValidateResult(SYUser user = null, bool success = false, ValidateResultError? error = null)
        {
            this.SYUser = user;
            this.Success = success;
            this.Error = error;
        }

        /// <summary>
        /// For additional data
        /// </summary>
        public object UserData { get; set; }
        public string ReturnURL { get; set; }
    }

    public enum ChangePasswordResultError
    {
        CredentialTypeNotFound,
        CredentialNotFound,
        PasswordLengthError,
        PasswordRuleError
    }

    public class ChangePasswordResult
    {
        public bool Success { get; set; }
        public ChangePasswordResultError? Error { get; set; }

        public ChangePasswordResult(bool success = false, ChangePasswordResultError? error = null)
        {
            this.Success = success;
            this.Error = error;
        }
    }

}
