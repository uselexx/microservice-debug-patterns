export interface MovieDto {
  id: number;
  legacyId: number;
  title: string;
  overview?: string;
  releaseDate: string;
  popularity: number;
}

export interface GetMovieResponse {
  correlationId: string;
  movie: MovieDto;
  error: string | null;
}

export interface PagedResponse<T> {
  data: T[];
  nextCursor: number | null;
  hasMore: boolean;
}

export interface GetMoviesResponse {
  movies: PagedResponse<GetMovieResponse>;
}