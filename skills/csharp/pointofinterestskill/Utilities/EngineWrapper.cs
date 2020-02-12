﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Bot.Expressions.Memory;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Responses;

namespace PointOfInterestSkill.Utilities
{
    public class EngineWrapper : LocaleTemplateEngineManager
    {
        public static readonly string PathBase = "../../Content/";

        public EngineWrapper(Dictionary<string, List<string>> localeLGFiles, string fallbackLocale)
            : base(localeLGFiles, fallbackLocale)
        {
        }

        public Activity GetCardResponse(Card card)
        {
            return GetCardResponse(new Card[] { card });
        }

        public Activity GetCardResponse(IEnumerable<Card> cards, string attachmentLayout = "carousel")
        {
            return GetCardResponse("CardsOnly", cards, null, attachmentLayout);
        }

        public Activity GetCardResponse(string templateId, Card card, StringDictionary tokens = null)
        {
            return GetCardResponse(templateId, new Card[] { card }, tokens);
        }

        public Activity GetCardResponse(string templateId, IEnumerable<Card> cards, StringDictionary tokens = null, string attachmentLayout = "carousel")
        {
            var input = new
            {
                Data = Convert(tokens),
                Cards = cards.Select((card) => { return Convert(card); }).ToArray(),
                Layout = attachmentLayout,
            };
            try
            {
                return GenerateActivityForLocale(templateId, input);
            }
            catch (Exception ex)
            {
                var result = Activity.CreateMessageActivity();
                result.Text = ex.Message;
                return (Activity)result;
            }
        }

        public Activity GetCardResponse(string templateId, Card card, StringDictionary tokens = null, string containerName = null, IEnumerable<Card> containerItems = null)
        {
            // throw new Exception("1. only keep body in containee;2. in the container, write @{join(foreach(Cards,Card,CreateStringNoContainer(Card.Name,Card.Data)),',')};");
            var input = new
            {
                Data = Convert(tokens),
                Cards = new CardExt[] { Convert(card, containerItems) },
            };
            try
            {
                return GenerateActivityForLocale(templateId, input);
            }
            catch (Exception ex)
            {
                var result = Activity.CreateMessageActivity();
                result.Text = ex.Message;
                return (Activity)result;
            }
        }

        public Activity GetResponse(string templateId, StringDictionary tokens = null)
        {
            return GetCardResponse(templateId, Array.Empty<Card>(), tokens);
        }

        public string GetString(string templateId)
        {
            return GenerateActivityForLocale(templateId + ".Text").Text;
        }

        private CardExt Convert(Card card, IEnumerable<Card> containerItems = null)
        {
            var res = new CardExt { Name = PathBase + card.Name + ".json", Data = card.Data };
            if (containerItems != null)
            {
                res.Cards = containerItems.Select((card) => Convert(card)).ToList();
            }
            return res;
        }

        public static IDictionary<string, string> Convert(StringDictionary tokens)
        {
            var dict = new Dictionary<string, string>();
            if (tokens != null)
            {
                foreach (DictionaryEntry key in tokens)
                {
                    dict[(string)key.Key] = (string)key.Value;
                }
            }

            return dict;
        }

        public class CardExt : Card
        {
            public List<CardExt> Cards { get; set; }
        }
    }
}
