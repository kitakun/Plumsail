export const AVAILABLE_TAGS = [
  'Skiing',
  'Hiking',
  'Gaming',
  'Painting',
  'Reading',
  'Cooking',
  'Photography',
  'Traveling',
  'Swimming',
  'Cycling',
  'Running',
  'Yoga',
  'Dancing',
  'Music',
  'Writing',
  'Drawing',
  'Gardening',
  'Fishing',
  'Camping',
  'Rock Climbing',
  'Surfing',
  'Tennis',
  'Basketball',
  'Soccer',
  'Volleyball',
  'Golf',
  'Chess',
  'Puzzles',
  'Knitting',
  'Sewing',
  'Woodworking',
  'Pottery',
  'Sculpting',
  'Singing',
  'Acting',
  'Theater',
  'Movies',
  'TV Shows',
  'Anime',
  'Manga',
  'Comics',
  'Board Games',
  'Video Games',
  'Programming',
  'Technology',
  'Science',
  'History',
  'Languages',
  'Meditation',
  'Fitness',
] as const;

export type AvailableTag = (typeof AVAILABLE_TAGS)[number];

export type SignUpRecord = {
  id: string;
  firstName: string;
  lastName: string;
  birthDate: string;
  gender: 'Male' | 'Female' | 'Other' | 'Prefer not to say';
  tags: AvailableTag[];
};

export type SignUpFormData = {
  firstName: string;
  lastName: string;
  birthDate: string;
  gender: 'Male' | 'Female' | 'Other' | 'Prefer not to say';
  tags: AvailableTag[];
};
