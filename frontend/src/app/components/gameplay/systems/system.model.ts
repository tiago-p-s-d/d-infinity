export interface SystemModel {
  id?: number;
  name: string;
  characterSheetId: number;
  itemsId: number;
  spellsId: number;
  skillsId: number;
  mapsId: number;
  currencyId: number;
  racesId: number;
  classesId?: number;
  skillTreeId?: number;
  createdBy?: number;
}