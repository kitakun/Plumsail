import type { SubmissionStatus, PriorityLevel } from '../submissions.types';

/**
 * Maps enum values to human-readable strings for Status
 */
export const getStatusLabel = (status: SubmissionStatus): string => {
  const statusMap: Record<SubmissionStatus, string> = {
    'Pending': 'Pending',
    'InReview': 'In Review',
    'Approved': 'Approved',
    'Rejected': 'Rejected'
  };
  return statusMap[status] || status;
};

/**
 * Maps enum values to human-readable strings for Priority
 */
export const getPriorityLabel = (priority: PriorityLevel): string => {
  const priorityMap: Record<PriorityLevel, string> = {
    'Low': 'Low',
    'Medium': 'Medium',
    'High': 'High',
    'Critical': 'Critical'
  };
  return priorityMap[priority] || priority;
};

