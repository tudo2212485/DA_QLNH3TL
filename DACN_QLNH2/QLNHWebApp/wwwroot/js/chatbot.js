// ========================================
// CHATBOT WIDGET JAVASCRIPT
// Modern chatbot with API integration
// ========================================

class ChatBot {
    constructor() {
        this.isOpen = false;
        this.messagesContainer = null;
        this.inputField = null;
        this.sendButton = null;
        this.init();
    }

    init() {
        // Tạo HTML structure
        this.createChatbotHTML();
        
        // Lấy references
        this.messagesContainer = document.getElementById('chatbot-messages');
        this.inputField = document.getElementById('chatbot-input');
        this.sendButton = document.getElementById('chatbot-send');
        
        // Bind events
        this.bindEvents();
        
        // Hiển thị welcome message
        this.addBotMessage(
            'Xin chào! 👋 Tôi là trợ lý ảo của nhà hàng. Tôi có thể giúp gì cho bạn?',
            [
                'Quán mở cửa lúc mấy giờ?',
                'Menu có những món gì?',
                'Địa chỉ nhà hàng ở đâu?',
                'Làm sao để đặt bàn?'
            ]
        );
    }

    createChatbotHTML() {
        const html = `
            <div id="chatbot-container">
                <!-- Floating Button -->
                <button id="chatbot-toggle" title="Chat với chúng tôi">
                    💬
                </button>
                
                <!-- Chat Window -->
                <div id="chatbot-window">
                    <!-- Header -->
                    <div id="chatbot-header">
                        <div>
                            <h3>🤖 Trợ lý ảo</h3>
                            <div class="status">● Online - Luôn sẵn sàng hỗ trợ</div>
                        </div>
                        <button id="chatbot-close">✕</button>
                    </div>
                    
                    <!-- Messages -->
                    <div id="chatbot-messages"></div>
                    
                    <!-- Input -->
                    <div id="chatbot-input-container">
                        <input 
                            type="text" 
                            id="chatbot-input" 
                            placeholder="Nhập câu hỏi của bạn..."
                            autocomplete="off"
                        />
                        <button id="chatbot-send" title="Gửi">
                            ➤
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        document.body.insertAdjacentHTML('beforeend', html);
    }

    bindEvents() {
        // Toggle chatbot
        document.getElementById('chatbot-toggle').addEventListener('click', () => {
            this.toggleChat();
        });
        
        // Close chatbot
        document.getElementById('chatbot-close').addEventListener('click', () => {
            this.closeChat();
        });
        
        // Send message on button click
        this.sendButton.addEventListener('click', () => {
            this.sendMessage();
        });
        
        // Send message on Enter key
        this.inputField.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.sendMessage();
            }
        });
    }

    toggleChat() {
        this.isOpen = !this.isOpen;
        const chatWindow = document.getElementById('chatbot-window');
        
        if (this.isOpen) {
            chatWindow.classList.add('active');
            this.inputField.focus();
        } else {
            chatWindow.classList.remove('active');
        }
    }

    closeChat() {
        this.isOpen = false;
        document.getElementById('chatbot-window').classList.remove('active');
    }

    async sendMessage() {
        const message = this.inputField.value.trim();
        
        if (!message) return;
        
        // Disable input
        this.inputField.disabled = true;
        this.sendButton.disabled = true;
        
        // Hiển thị user message
        this.addUserMessage(message);
        
        // Clear input
        this.inputField.value = '';
        
        // Hiển thị typing indicator
        this.showTypingIndicator();
        
        try {
            // Gọi API
            const response = await fetch('/api/ChatBotApi/message', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ message: message })
            });
            
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            
            const data = await response.json();
            
            // Remove typing indicator
            this.removeTypingIndicator();
            
            // Hiển thị bot response
            this.addBotMessage(data.message, data.suggestions);
            
        } catch (error) {
            console.error('Chatbot error:', error);
            
            // Remove typing indicator
            this.removeTypingIndicator();
            
            // Hiển thị error message
            this.addBotMessage(
                '❌ Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại hoặc gọi hotline: 0901 234 567',
                ['Thử lại', 'Xem menu', 'Liên hệ']
            );
        } finally {
            // Enable input
            this.inputField.disabled = false;
            this.sendButton.disabled = false;
            this.inputField.focus();
        }
    }

    addUserMessage(text) {
        const messageHTML = `
            <div class="chat-message user">
                <div class="message-bubble">
                    ${this.escapeHtml(text)}
                </div>
            </div>
        `;
        
        this.messagesContainer.insertAdjacentHTML('beforeend', messageHTML);
        this.scrollToBottom();
    }

    addBotMessage(text, suggestions = []) {
        // Convert markdown-like syntax to HTML
        const formattedText = this.formatMessage(text);
        
        let suggestionsHTML = '';
        if (suggestions && suggestions.length > 0) {
            suggestionsHTML = `
                <div class="chat-suggestions">
                    ${suggestions.map(s => `
                        <button class="suggestion-btn" onclick="chatBot.handleSuggestion('${this.escapeHtml(s)}')">${this.escapeHtml(s)}</button>
                    `).join('')}
                </div>
            `;
        }
        
        const messageHTML = `
            <div class="chat-message bot">
                <div class="bot-avatar">🤖</div>
                <div>
                    <div class="message-bubble">
                        ${formattedText}
                    </div>
                    ${suggestionsHTML}
                </div>
            </div>
        `;
        
        this.messagesContainer.insertAdjacentHTML('beforeend', messageHTML);
        this.scrollToBottom();
    }

    showTypingIndicator() {
        const indicatorHTML = `
            <div class="chat-message bot" id="typing-indicator">
                <div class="bot-avatar">🤖</div>
                <div class="message-bubble">
                    <div class="typing-indicator">
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                </div>
            </div>
        `;
        
        this.messagesContainer.insertAdjacentHTML('beforeend', indicatorHTML);
        this.scrollToBottom();
    }

    removeTypingIndicator() {
        const indicator = document.getElementById('typing-indicator');
        if (indicator) {
            indicator.remove();
        }
    }

    handleSuggestion(text) {
        this.inputField.value = text;
        this.sendMessage();
    }

    formatMessage(text) {
        // Convert **bold** to <strong>
        text = text.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
        
        // Convert line breaks
        text = text.replace(/\n/g, '<br>');
        
        return text;
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    scrollToBottom() {
        setTimeout(() => {
            this.messagesContainer.scrollTop = this.messagesContainer.scrollHeight;
        }, 100);
    }
}

// Initialize chatbot when DOM is ready
let chatBot;

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        chatBot = new ChatBot();
    });
} else {
    chatBot = new ChatBot();
}
