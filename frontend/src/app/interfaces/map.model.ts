export interface CampaignMap {
  id: number;
  name: string;
  mapImage: string;
  createdBy: number;
  mapGroupId: number;
  gridCellSize?: number; 
  mapGroup?: {
    id: number;
    name: string;
    createdBy: number;
  };
}