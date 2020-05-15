import { Photo } from './photo';

export interface User {
  id: number;
  username: string;
  knownAs: string;
  age: number;
  gender: string;
  created: Date;
  lastActive: Date;
  photoUrl: string;
  city: string;
  country: string;
  //Optional properties (?) MUST follow the required ones
  interests?: string;
  introduction?: string;
  lookingFor?: string;
  photos?: Photo[];
}
