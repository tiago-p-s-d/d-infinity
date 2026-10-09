export interface MapToken {
  id: number;
  name: string;
  tokenImage: string;
  coordX: number;
  coordY: number;
  size: number;
  mapId: number;
  characterSheetId: number;
  ownerUserId: number;
}

export interface CreateMapTokenDto {
  name: string;
  tokenImage: string;
  mapId: number;
  characterSheetId: number;
  ownerUserId: number;
  coordX?: number;
  coordY?: number;
  size?: number;
}