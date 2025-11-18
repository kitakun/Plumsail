<template>
  <div class="messanger-page">
    <main>
      <div class="messanger-container">
        <h2 class="page-title">Messanger</h2>
        
        <div class="messages-container">
          <div v-if="loadingMessages" class="loading-messages">Loading messages...</div>
          <div v-else-if="messagesError" class="error-message">{{ messagesError }}</div>
          <div v-else-if="messages.length === 0" class="no-messages">No messages yet. Be the first to send one!</div>
          <div v-else class="messages-list">
            <div v-for="message in messages" :key="message.id" class="message-item">
              <div class="message-text">{{ message.text }}</div>
              <div v-if="message.createdDate" class="message-date">
                {{ formatDate(message.createdDate) }}
              </div>
            </div>
          </div>
        </div>

        <div class="messanger-form">
          <div class="input-group">
            <input
              v-model="messageText"
              type="text"
              placeholder="Type your message here..."
              class="message-input"
              @keyup.enter="sendMessage"
            />
            <button class="send-button" @click="sendMessage" :disabled="loading || !messageText.trim()">
              <span v-if="loading">Sending...</span>
              <span v-else>Send</span>
            </button>
          </div>
          <div v-if="error" class="error-message">{{ error }}</div>
          <div v-if="success" class="success-message">Message sent successfully!</div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { messangerApi, type Message } from './services/messanger.api';
import { formatDate } from './utils/dateUtils';

const messageText = ref('');
const loading = ref(false);
const error = ref<string | null>(null);
const success = ref(false);
const messages = ref<Message[]>([]);
const loadingMessages = ref(false);
const messagesError = ref<string | null>(null);

const loadMessages = async () => {
  loadingMessages.value = true;
  messagesError.value = null;

  try {
    const response = await messangerApi.getMessages(0, 100); // Load up to 100 messages
    if (response.isSuccess && response.result) {
      messages.value = response.result.items;
    } else {
      messagesError.value = response.exception || 'Failed to load messages';
    }
  } catch (err) {
    messagesError.value = 'An error occurred while loading messages';
    console.error(err);
  } finally {
    loadingMessages.value = false;
  }
};

const sendMessage = async () => {
  if (!messageText.value.trim()) {
    return;
  }

  loading.value = true;
  error.value = null;
  success.value = false;

  try {
    const response = await messangerApi.sendMessage({ text: messageText.value.trim() });
    if (response.isSuccess) {
      success.value = true;
      messageText.value = '';
      setTimeout(() => {
        success.value = false;
      }, 3000);
      await loadMessages();
    } else {
      error.value = response.exception || 'Failed to send message';
    }
  } catch (err) {
    error.value = 'An error occurred while sending the message';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadMessages();
});
</script>

<style scoped>
.messanger-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
  padding: 2rem;
  display: flex;
  justify-content: center;
  align-items: flex-start;
}

.messanger-container {
  max-width: 600px;
  width: 100%;
  background-color: #2a2a2a;
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.page-title {
  margin: 0 0 2rem 0;
  font-size: 2rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.87);
  text-align: center;
}

.messages-container {
  max-height: 400px;
  overflow-y: auto;
  margin-bottom: 2rem;
  padding: 1rem;
  background-color: #1e1e1e;
  border-radius: 4px;
  border: 1px solid #444;
}

.messages-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.message-item {
  padding: 0.75rem;
  background-color: #2a2a2a;
  border-radius: 4px;
  border-left: 3px solid #42b983;
}

.message-text {
  color: rgba(255, 255, 255, 0.87);
  font-size: 0.95rem;
  line-height: 1.5;
  word-wrap: break-word;
}

.message-date {
  margin-top: 0.5rem;
  font-size: 0.75rem;
  color: #888;
}

.loading-messages,
.no-messages {
  text-align: center;
  padding: 2rem;
  color: #888;
  font-style: italic;
}

.messanger-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.input-group {
  display: flex;
  gap: 1rem;
}

.message-input {
  flex: 1;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  border: 2px solid #444;
  border-radius: 4px;
  background-color: #1e1e1e;
  color: rgba(255, 255, 255, 0.87);
  transition: border-color 0.3s;
}

.message-input:focus {
  outline: none;
  border-color: #42b983;
}

.message-input::placeholder {
  color: #888;
}

.send-button {
  padding: 0.75rem 2rem;
  font-size: 1rem;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.3s;
  white-space: nowrap;
}

.send-button:hover:not(:disabled) {
  background-color: #35a372;
}

.send-button:active:not(:disabled) {
  transform: scale(0.98);
}

.send-button:disabled {
  background-color: #666;
  cursor: not-allowed;
  opacity: 0.6;
}

.error-message {
  padding: 0.75rem;
  background-color: #e74c3c;
  color: white;
  border-radius: 4px;
  font-size: 0.9rem;
}

.success-message {
  padding: 0.75rem;
  background-color: #42b983;
  color: white;
  border-radius: 4px;
  font-size: 0.9rem;
}

@media (prefers-color-scheme: light) {
  .messanger-container {
    background-color: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .page-title {
    color: #213547;
  }

  .messages-container {
    background-color: #f5f5f5;
    border: 1px solid #ddd;
  }

  .message-item {
    background-color: white;
    border-left-color: #42b983;
  }

  .message-text {
    color: #213547;
  }

  .message-date {
    color: #666;
  }

  .loading-messages,
  .no-messages {
    color: #666;
  }

  .message-input {
    border: 2px solid #ddd;
    background-color: white;
    color: #213547;
  }

  .message-input::placeholder {
    color: #999;
  }
}

@media (max-width: 768px) {
  .input-group {
    flex-direction: column;
  }

  .send-button {
    width: 100%;
  }
}
</style>

