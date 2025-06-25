-- Tạo database
CREATE DATABASE IF NOT EXISTS `waifu_ai_assistant`
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

USE `waifu_ai_assistant`;

-- Tắt các kiểm tra để tiện tạo bảng
SET FOREIGN_KEY_CHECKS = 0;

-- Xoá các bảng nếu đã tồn tại
DROP TABLE IF EXISTS feedback;
DROP TABLE IF EXISTS messages;
DROP TABLE IF EXISTS sessions;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS chatbots;
DROP TABLE IF EXISTS settings;

-- Bảng người dùng
CREATE TABLE users (
  userId BINARY(16) NOT NULL PRIMARY KEY,
  username VARCHAR(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Bảng phiên trò chuyện (session)
CREATE TABLE sessions (
  sessionId BINARY(16) NOT NULL PRIMARY KEY,
  userId BINARY(16) NOT NULL,
  started_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  ended_at DATETIME DEFAULT NULL,
  FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Bảng tin nhắn
CREATE TABLE messages (
  messageId BINARY(16) NOT NULL PRIMARY KEY,
  sessionId BINARY(16) NOT NULL,
  sender ENUM('user', 'bot') NOT NULL,
  content TEXT NOT NULL,
  sent_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (sessionId) REFERENCES sessions(sessionId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Bảng góp ý người dùng
CREATE TABLE feedback (
  feedbackId BINARY(16) NOT NULL PRIMARY KEY,
  userId BINARY(16) NOT NULL,
  messageId BINARY(16),
  rating TINYINT, -- 1 đến 5 sao
  comment TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE,
  FOREIGN KEY (messageId) REFERENCES messages(messageId) ON DELETE SET NULL
) ENGINE=InnoDB;

-- Bảng cấu hình chatbot
CREATE TABLE chatbots (
  botId BINARY(16) NOT NULL PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description TEXT,
  temperature FLOAT DEFAULT 0.7,
  model VARCHAR(100) DEFAULT 'gpt-4o',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Bảng cài đặt tuỳ chọn người dùng
CREATE TABLE settings (
  settingId BINARY(16) NOT NULL PRIMARY KEY,
  userId BINARY(16) NOT NULL,
  preferred_model VARCHAR(100) DEFAULT 'gpt-4o',
  language VARCHAR(10) DEFAULT 'en',
  theme VARCHAR(10) DEFAULT 'light',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Bật lại kiểm tra khóa ngoại
SET FOREIGN_KEY_CHECKS = 1;

-- Chèn dữ liệu mẫu (tuỳ chọn)
INSERT INTO users (userId, username) VALUES (UUID_TO_BIN(UUID()), 'Tùng');
