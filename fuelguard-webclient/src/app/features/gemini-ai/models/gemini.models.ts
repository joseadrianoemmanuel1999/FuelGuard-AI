export interface GeminiChatRequestDto {
  question: string;
  stationHint?: string | null;
  periodHint?: string | null;
  additionalContext?: string | null;
}

export interface GeminiChatResponseDto {
  answer: string;
  riskLevel: string;
  recommendations: string[];
  explainability: string;
  poweredByGemini: boolean;
  disclaimer?: string | null;
}

export interface GeminiInsightsResponseDto {
  operationalInsights: string[];
  anomalyHighlights: string[];
  recommendations: string[];
  executiveSummary: string;
  riskObservations: string[];
  poweredByGemini: boolean;
  disclaimer?: string | null;
}

export interface GeminiChatMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  riskLevel?: string;
  recommendations?: string[];
  explainability?: string;
  poweredByGemini?: boolean;
  isTyping?: boolean;
}
