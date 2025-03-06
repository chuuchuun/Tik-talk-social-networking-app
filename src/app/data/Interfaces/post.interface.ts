export interface Post {
    id: number;
    title: string;
    communityId: number | null;
    content: string | null;
    authorId: number;
    images: string[] | null;
    createdAt: Date;
    updatedAt: Date;
    likes: number;
    comments: string[] | null;
}
