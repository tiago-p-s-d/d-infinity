export interface CampaignDetails {
  id: number;
  campaignName: string;
  about?: string;
  isDm: boolean;
  system: {
    id: number;
    name: string;
  };
}