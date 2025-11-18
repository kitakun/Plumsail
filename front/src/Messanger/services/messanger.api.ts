import axios from 'axios';
import type { OperationResult, Pagination } from '../../types/api';
import type { FileRecord } from '../../Submissions/submissions.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export type MessageRequest = {
  text: string;
};

export type Message = {
  id: string;
  text: string;
  createdDate?: string;
};

export const messangerApi = {
  async sendMessage(message: MessageRequest): Promise<OperationResult<void>> {
    const response = await apiClient.put<OperationResult<void>>('/submission', message);
    return response.data;
  },

  async getMessages(offset?: number, limit?: number): Promise<OperationResult<Pagination<Message>>> {
    const params = new URLSearchParams();
    if (offset !== undefined) params.append('offset', offset.toString());
    if (limit !== undefined) params.append('limit', limit.toString());

    const response = await apiClient.get<OperationResult<Pagination<FileRecord>>>(
      `/submission?${params.toString()}`
    );

    if (!response.data.isSuccess || !response.data.result) {
      return {
        isSuccess: false,
        exception: response.data.exception || 'Failed to load messages',
      } as OperationResult<Pagination<Message>>;
    }

    // Filter to only messages (submissions with text property and no file content)
    const messages: Message[] = response.data.result.items
      .filter((record: FileRecord) => {
        // Only include records with no file content (size = 0) and have text in payload
        return (
          record.fileData.size === 0 &&
          record.payload &&
          typeof record.payload.text === 'string' &&
          record.payload.text.trim() !== ''
        );
      })
      .map((record: FileRecord) => ({
        id: record.id,
        text: record.payload.text as string,
        createdDate: record.payload.CreatedDate as string | undefined,
      }))
      .sort((a, b) => {
        // Sort by date: newest first (descending)
        if (!a.createdDate && !b.createdDate) return 0;
        if (!a.createdDate) return 1; // Messages without date go to the end
        if (!b.createdDate) return -1;
        return new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime();
      });

    return {
      isSuccess: true,
      result: {
        items: messages,
        totalCount: messages.length,
      },
    };
  },
};

