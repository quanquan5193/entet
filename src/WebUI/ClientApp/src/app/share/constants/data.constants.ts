export const DATA = {
  CardStatusDropdown: [
    {
      Value: '1',
      Text: '未発行'
    },
    {
      Value: '2',
      Text: '発行済'
    },
    {
      Value: '3',
      Text: '退会'
    },
    {
      Value: '4',
      Text: '紛失'
    },
    {
      Value: '5',
      Text: '廃棄'
    }
  ],
  CardStatus: {
    Unissued: 1,
    Issued: 2,
    Withdrawal: 3,
    Missing: 4,
    Disposal: 5
  },
  SexType: {
    Male: 1,
    Female: 2,
    Other: 3
  },
  KidRelationship: {
    Unset: 0,
    Father: 1,
    Mother: 2,
    GrandFarther: 3,
    GrandMother: 4,
    Other: 5,
  },
  DisplayChart: {
    CreateCard: "プリカ新規",
    SwitchCard: "カード切替",
    KidClub: "キッズ新規",
    Other: "その他",
  },
};
