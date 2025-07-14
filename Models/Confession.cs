// Whisper/Models/Confession.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Whisper.Models
{
    public class Confession
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the confession

        [Required(ErrorMessage = "Confession text is required.")]
        [MinLength(10, ErrorMessage = "Confession must be at least 10 characters long.")]
        [MaxLength(500, ErrorMessage = "Confession cannot exceed 500 characters.")]
        public string ConfessionText { get; set; } = string.Empty; // Initialized to empty string

        public string AdviceText { get; set; } = string.Empty; // Initialized to empty string

        public bool IsPublishedAnonymously { get; set; } = false; // Flag to check if published

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // When the confession was made
    }
} 